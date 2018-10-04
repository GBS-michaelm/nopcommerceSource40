using GBSShippingRateConvert;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.DataAccess.GBS;
using Nop.Plugin.Shipping.GBS.Models.Product;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using WebServices.Models;

namespace Nop.Plugin.Shipping.GBS.Infrastructure
{ /// <summary>
  /// Fixed rate shipping computation method
  /// </summary>
    public class GBSShippingComputationPlugin : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly GBSShippingSetting _gbsShippingSetting;
        private readonly IWebHelper _webHelper;

        public string _TableName = "";
        #endregion

        #region Ctor
        public GBSShippingComputationPlugin(IShippingService shippingService, ISettingService settingService,
            ILocalizationService localizationService, 
            IWorkContext workContext,
            GBSShippingSetting gbsShippingSetting,
            IWebHelper webHelper)
        {
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._gbsShippingSetting = gbsShippingSetting;
            this._webHelper = webHelper;

        }
        #endregion

        #region Methods

        public override void Install()
        {   
            // Add Local String Resources for this plugin
            LocaleRes();

            base.Install();
        }

        public override void Uninstall()
        {
            // Delete Local String Resources for this plugin
            UnLocaleRes();

            base.Uninstall();
        }


        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            //if (getShippingOptionRequest == null)
            //    throw new ArgumentNullException("getShippingOptionRequest");

            decimal? _FixedRate = null;
            if (_gbsShippingSetting.UseFlatRate)
            {
                _FixedRate = _gbsShippingSetting.FlatRateAmount;
            }
            return _FixedRate;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            _TableName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Table.Name").ResourceValue.ToString();

            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("No shipment items");
                return response;
            }
            
            int? restrictByCountryId = (getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null) ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
            var shippingMethods = this._shippingService.GetAllShippingMethods(restrictByCountryId);
            IEnumerable<int> productId = getShippingOptionRequest.Items.Select(x => x.ShoppingCartItem.ProductId).Distinct();
            ShippingRateConversionModels _lstOfProduct = new ShippingRateConversionModels();
            foreach (var pID in productId)
            {
                ProductGBSModel _prodModel = DataManager.GetGBSShippingCategory(pID, _TableName);
                ShippingRateConversionModels.ProductGBSModel _productGBSModel = new ShippingRateConversionModels.ProductGBSModel();

                _productGBSModel.ProductID = _prodModel.ProductID.ToString();
                _productGBSModel.ShippingCategoryA = _prodModel.ShippingCategoryA;
                _productGBSModel.ShippingCategoryB = _prodModel.ShippingCategoryB;

                _lstOfProduct.productGBSModels.Add(_productGBSModel);
            }

            // Get ZipCode and Product/s ID/S
            var zipCode = (getShippingOptionRequest.ShippingAddress != null && !String.IsNullOrEmpty(getShippingOptionRequest.ShippingAddress.ZipPostalCode)) ? getShippingOptionRequest.ShippingAddress.ZipPostalCode.Substring(0, 3) : String.Empty; 
            // Get Categorized Value from String Resources
            _lstOfProduct.zipCode = zipCode;
            // Call Web Service to get the Shipping rate
            GBSShippingServiceClient myShippingRateSrv = new GBSShippingServiceClient();

            //check for Flat Rate
            string _FixedRate = String.Empty;
            if (_gbsShippingSetting.UseFlatRate)
            {
                _FixedRate = _gbsShippingSetting.FlatRateAmount.ToString();
            } else
            {
                _FixedRate = myShippingRateSrv.GetShippingRate(_gbsShippingSetting.GBSShippingWebServiceAddress, _gbsShippingSetting.LoginId, _gbsShippingSetting.Password, _lstOfProduct);
            }


            // Build response with custom shipping option 
            try
            {
                //see if we have a parsable string
               decimal parsedRate = decimal.Parse(_FixedRate);
            }
            catch (Exception e)
            {
                _FixedRate = "0.00";
            }
            decimal? maxRate = decimal.Parse(_FixedRate);
            var shippingOption = new ShippingOption();
            var shipOpName = _localizationService.GetResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name");
            shippingOption.Name = shipOpName.ToString();
            shippingOption.Rate = maxRate.HasValue == true ? maxRate.Value : 0 ;
            var shipOpDesError = _localizationService.GetResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error");
            var shipOpDesSuccess = _localizationService.GetResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success");
            shippingOption.Description = shippingOption.Rate == 0 ? shipOpDesError : shipOpDesSuccess ;

            response.ShippingOptions.Add(shippingOption);
            return response;
        }

        #endregion

        #region Properties

        public override string GetConfigurationPageUrl() {
            return _webHelper.GetStoreLocation() + "Admin/ExtendedFields/Configure";
        }

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Offline;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get
            {
                //uncomment a line below to return a general shipment tracker (finds an appropriate tracker by tracking number)
                //return new GeneralShipmentTracker(EngineContext.Current.Resolve<ITypeFinder>());
                return null;
            }
        }

        #endregion

        #region Helper

        // Add String Resources 
        public void LocaleRes()
        {
            // Add Local String Resocurces for the Admin Side Extended Field
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryA", "Configure :: Shipping Category Field Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryA.Hint", "Add Shipping Category A Value");

            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryB", "Configure :: Shipping Category Field Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryB.Hint", "Add Shipping Category B Value");

            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error" , "Configure :: Error Message :");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success", "Configure :: Success Message :");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name", "Configure :: Description Name :");

            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.GBS.Product.Table.Name" , "Configure :: TABLE NAME");

           }

        // Delete String Resources
        public void UnLocaleRes()
        {
            // Del Local String Resocurces for the Admin Side Extended Field
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryA");
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryA.Hint");

            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryB");
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingCategoryB.Hint");

            // Delete Local String Resources for this plugin
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error");
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success");
            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name");

            this.DeletePluginLocaleResource("Plugins.Shipping.GBS.Product.Table.Name");
     
        }

        #endregion
    }
}


