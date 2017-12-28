using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Nop.Web.Factories;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Checkout.GBS.Controllers
{
    [NopHttpsRequirement(SslRequirement.Yes)]
    public partial class CheckoutController : BasePublicController
    {
        private Nop.Web.Controllers.CheckoutController _baseNopCheckoutController;
        private Nop.Web.Controllers.ShoppingCartController _baseNopShoppingCartController;
        #region Fields

        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IDateRangeService _dateRangeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressModelFactory _addressModelFactory;

        #endregion Fields

        #region Constructors

        public CheckoutController(
            IShoppingCartModelFactory shoppingCartModelFactory,
            IProductService productService,
            IWorkContext workContext, 
            IPictureService pictureService,
            ICheckoutModelFactory checkoutModelFactory,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IShoppingCartService shoppingCartService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IPriceCalculationService priceCalculationService,
            ICheckoutAttributeParser checkoutAttributeParser,
             IDiscountService discountService,
              IGiftCardService giftCardService,
              ICheckoutAttributeService checkoutAttributeService,
              MediaSettings mediaSettings,
              TaxSettings taxSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            CaptchaSettings captchaSettings,
            IDateRangeService dateRangeService,


            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICustomerActivityService customerActivityService,
            IWorkflowMessageService workflowMessageService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICacheManager cacheManager,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IPaymentService paymentService,
            IPluginFinder pluginFinder,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            ILogger logger,
            IOrderService orderService,
            IWebHelper webHelper,
            HttpContextBase httpContext,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._checkoutModelFactory = checkoutModelFactory;
            this._dateRangeService = dateRangeService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._shoppingCartService = shoppingCartService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;
            this._logger = logger;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;

            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._customerSettings = customerSettings;
            this._addressModelFactory = addressModelFactory;
            this._baseNopCheckoutController = new Nop.Web.Controllers.CheckoutController(
                this._checkoutModelFactory,
                this._workContext, 
                this._storeContext, 
                //this._storeMappingService, 
                this._shoppingCartService, 
                this._localizationService,
                //this._taxService, 
                //this._currencyService, 
                //this._priceFormatter, 
                this._orderProcessingService, 
                this._customerService, 
                this._genericAttributeService, 
                this._countryService, 
                this._stateProvinceService,
                this._shippingService, 
                this._paymentService, 
                this._pluginFinder, 
                this._orderTotalCalculationService, 
                //this._rewardPointService, 
                this._logger, 
                this._orderService, 
                this._webHelper,
                this._httpContext, 
                this._addressAttributeParser, 
                this._addressAttributeService, 
                //this._addressAttributeFormatter, 
                this._orderSettings,
                this._rewardPointsSettings, 
                this._paymentSettings, 
                this._shippingSettings, 
                this._addressSettings, 
                this._customerSettings);
            this._baseNopShoppingCartController = new Nop.Web.Controllers.ShoppingCartController(
                this._shoppingCartModelFactory,
                productService,
                this._storeContext,
                this._workContext,
                this._shoppingCartService,
                pictureService,
                this._localizationService,
                productAttributeService,
                //productAttributeFormatter,
                productAttributeParser,
                this._taxService,
                this._currencyService,
                priceCalculationService,
                this._priceFormatter,
                checkoutAttributeParser,
                //checkoutAttributeFormatter,
                //this._orderProcessingService,
                discountService,
                this._customerService,
                giftCardService,
                this._dateRangeService,
                //this._countryService,
                //this._stateProvinceService,
                //this._shippingService,
                //this._orderTotalCalculationService,
                checkoutAttributeService,
                //this._paymentService,
                workflowMessageService,
                permissionService,
                downloadService,
                cacheManager,
                this._webHelper,
                customerActivityService,
                this._genericAttributeService,
                //this._addressAttributeFormatter,
                //this._httpContext,
                mediaSettings,
                shoppingCartSettings,
                //catalogSettings,
                this._orderSettings,
                //this._shippingSettings,
                //taxSettings,
                captchaSettings,
                //this._addressSettings,
                //this._rewardPointsSettings,
                this._customerSettings
                );
        }

        #endregion Constructors

        public ActionResult Index()
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                ActionResult baseAR = _baseNopCheckoutController.Index();
                return RedirectToRoute("CheckoutShippingAddress");
            }
            return _baseNopCheckoutController.Index();
        }
        #region Shipping ActionResults

        public ActionResult ShippingAddress()
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
               .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();

                if (!cart.RequiresShipping())
                {
                    _workContext.CurrentCustomer.ShippingAddress = null;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    return RedirectToRoute("CheckoutShippingMethod");
                }

                //model
                var model = PrepareShippingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);



                return View(model);
            }return _baseNopCheckoutController.ShippingAddress();
        }

        public ActionResult SelectShippingAddress(int addressId, string shipType)
        {
            TempData["ShippingAddressId"] = addressId;
            TempData["ShippingType"] = shipType;

            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                ActionResult selectShipping = _baseNopCheckoutController.SelectShippingAddress(addressId);

                if (!string.IsNullOrEmpty(shipType))
                {
                    ActionResult selectShippingBase = _baseNopCheckoutController.SelectShippingMethod(shipType);
                }
                else
                {
                    return RedirectToRoute("CheckoutShippingMethod");
                }                
                
                return RedirectToRoute("CheckoutPaymentMethod");
            }
            return _baseNopCheckoutController.SelectShippingAddress(addressId);
        }

        [HttpPost, ActionName("ShippingAddress")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult NewShippingAddress(CheckoutShippingAddressModel model, FormCollection form)
        {

            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                //validation

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();

                if (!cart.RequiresShipping())
                {
                    _workContext.CurrentCustomer.ShippingAddress = null;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    return RedirectToRoute("CheckoutShippingMethod");
                }

                //pickup point
                if (_shippingSettings.AllowPickUpInStore)
                {
                    if (model.PickUpInStore)
                    {
                        //no shipping address selected
                        _workContext.CurrentCustomer.ShippingAddress = null;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        _httpContext.Session["PickUpInStore"] = model.PickUpInStore.ToString();

                        var pickupPoint = form["pickup-points-id"].Split(new[] { "___" }, StringSplitOptions.None);
                        var pickupPoints = _shippingService
                            .GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, _workContext.CurrentCustomer, pickupPoint[1], _storeContext.CurrentStore.Id).PickupPoints.ToList();
                        var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));
                        if (selectedPoint == null)
                            return RedirectToRoute("CheckoutShippingAddress");

                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                        };

                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, pickUpInStoreShippingOption, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, selectedPoint, _storeContext.CurrentStore.Id);

                        TempData["ShippingAddressId"] = "LocalPickup";

                        return RedirectToRoute("CheckoutPaymentMethod");
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                }

                //custom address attributes
                var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
                var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                foreach (var error in customAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }

                Match match = null;

                if (!string.IsNullOrEmpty(model.NewAddress.ZipPostalCode))
                {
                    Regex regex = new Regex(@"^\d{5}-\d{4}|\d{5}|[A-Z]\d[A-Z] \d[A-Z]\d$");
                    match = regex.Match(model.NewAddress.ZipPostalCode);
                    if (!match.Success)
                    {
                        ModelState.AddModelError("NewAddress.ZipPostalCode", "Not Valid Zip Code Format");
                    }
                }
                

                if (ModelState.IsValid)
                {
                    
                    model.NewAddress.ZipPostalCode = match.ToString();
                                   
                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;
                        _workContext.CurrentCustomer.Addresses.Add(address);
                    }
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    _baseNopCheckoutController.SelectShippingMethod(form["shipType"]);

                    
                        TempData["ShippingAddressId"] = address.Id;
                    
                    
                    return RedirectToRoute("CheckoutShippingAddress");
                    //return RedirectToRoute("CheckoutPaymentMethod");
                }


                //If we got this far, something failed, redisplay form

                model = PrepareShippingAddressModel(cart,
                    selectedCountryId: model.NewAddress.CountryId,
                    overrideAttributesXml: customAttributes);
                return View(model);


                //return _baseNopCheckoutController.NewShippingAddress(model, form);

            }

            ActionResult retVal = _baseNopCheckoutController.NewShippingAddress(model, form);
            //foreach (var item in _baseNopCheckoutController.ModelState)
            //{
            //    if (!ModelState.ContainsKey(item.Key))
            //    {
            //        ModelState.Add(item.Key, item.Value);
            //    }
            //}
            if (_baseNopCheckoutController.ModelState.IsValid && ModelState.IsValid)
            {
                return retVal;
            }

            //If we got this far, something failed, redisplay form
            var customAttributesOuter = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            model = _checkoutModelFactory.PrepareShippingAddressModel(
                selectedCountryId: model.NewAddress.CountryId,
                overrideAttributesXml: customAttributesOuter);
            return View(model);
        }

        #endregion

        #region Payment ActionResults

        public ActionResult PaymentInfo()
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    return RedirectToRoute("CheckoutConfirm");
                }

                //load payment method
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("CheckoutPaymentMethod");

                //Check whether payment info should be skipped
                if (paymentMethod.SkipPaymentInfo)
                {
                    //skip payment info page
                    var paymentInfo = new ProcessPaymentRequest();
                    //session save
                    _httpContext.Session["OrderPaymentInfo"] = paymentInfo;

                    return RedirectToRoute("CheckoutConfirm");
                }

                var model = PreparePaymentInfoModel(cart, paymentMethod);
                return View(model);
            }
            return _baseNopCheckoutController.PaymentInfo();
        }

        [HttpPost, ActionName("PaymentInfo")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult EnterPaymentInfo(FormCollection form)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {


                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    return RedirectToRoute("CheckoutConfirm");
                }

                //load payment method
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    return RedirectToRoute("CheckoutPaymentMethod");

                var paymentControllerType = paymentMethod.GetControllerType();
                var paymentController = DependencyResolver.Current.GetService(paymentControllerType) as BasePaymentController;
                if (paymentController == null)
                    throw new Exception("Payment controller cannot be loaded");

                var warnings = paymentController.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);



                //model
                var model = PreparePaymentInfoModel(cart, paymentMethod);



                if (ModelState.IsValid)
                {
                    var billingModel = PrepareBillingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);
                    //get payment info
                    var paymentInfo = paymentController.GetPaymentInfo(form);
                    //session save
                    _httpContext.Session["OrderPaymentInfo"] = paymentInfo;

                    /* Billing Address Set To Shipping */

                    switch (form["billingType"].ToString())
                    {
                        case "Same Address":
                            SelectBillingAddress(_workContext.CurrentCustomer.ShippingAddress.Id, false);
                            break;
                        case "Bill to this address":
                            int aid;
                            if (!string.IsNullOrEmpty(form["billingID"]))
                            {
                                aid = Int32.Parse(form["billingID"]);
                                SelectBillingAddress(aid, true);
                            }
                            break;
                        case "New Address":
                            return RedirectToRoute("CheckoutBillingAddress");
                        default:
                            break;
                    }

                    return RedirectToRoute("CheckoutConfirm");
                }
                //If we got this far, something failed, redisplay form
                return View(model);
            }
            return _baseNopCheckoutController.EnterPaymentInfo(form);
        }

        #endregion

        #region Billing ActionResults

        public ActionResult BillingAddress(FormCollection form)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {


                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
                if (!cart.Any())
                    return RedirectToRoute("ShoppingCart");

                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");

                if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                    return new HttpUnauthorizedResult();

                //model
                var model = PrepareBillingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);

                //check whether "billing address" step is enabled
                if (_orderSettings.DisableBillingAddressCheckoutStep)
                {
                    if (model.ExistingAddresses.Any())
                    {
                        //choose the first one
                        return SelectBillingAddress(model.ExistingAddresses.First().Id);
                    }

                    TryValidateModel(model);
                    TryValidateModel(model.NewAddress);
                    return NewBillingAddress(model, form);
                }

                return View(model);
            }
            return _baseNopCheckoutController.BillingAddress(form);
        }

        public ActionResult SelectBillingAddress(int addressId, bool shipToSameAddress = false)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
                if (address == null)
                    return RedirectToRoute("CheckoutConfirm");

                _workContext.CurrentCustomer.BillingAddress = address;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

                //ship to the same address?
                if (_shippingSettings.ShipToSameAddress && shipToSameAddress && cart.RequiresShipping())
                {
                    _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    //reset selected shipping method (in case if "pick up in store" was selected)
                    _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                    _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                    //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available)
                    return RedirectToRoute("CheckoutConfirm");
                }

                return RedirectToRoute("CheckoutConfirm");
            }
            return _baseNopCheckoutController.SelectBillingAddress(addressId, shipToSameAddress);
        }

        [HttpPost, ActionName("BillingAddress")]
        [FormValueRequired("nextstep")]
        [ValidateInput(false)]
        public ActionResult NewBillingAddress(CheckoutBillingAddressModel model, FormCollection form)
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("CheckoutOnePage");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return new HttpUnauthorizedResult();

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

                if (ModelState.IsValid)
                {
                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;
                        _workContext.CurrentCustomer.Addresses.Add(address);
                    }
                    _workContext.CurrentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                    //ship to the same address?
                    if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && cart.RequiresShipping())
                    {
                        _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        //reset selected shipping method (in case if "pick up in store" was selected)
                        _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                        //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                        return RedirectToRoute("CheckoutConfirm");
                    }

                    return RedirectToRoute("CheckoutConfirm");

                }

                //If we got this far, something failed, redisplay form
                model = PrepareBillingAddressModel(cart,
                    selectedCountryId: model.NewAddress.CountryId,
                    overrideAttributesXml: customAttributes);
                return View(model);
            }
            ActionResult retVal = _baseNopCheckoutController.NewBillingAddress(model, form);

            if (_baseNopCheckoutController.ModelState.IsValid && ModelState.IsValid)
            {
                return retVal;
            }

            //If we got this far, something failed, redisplay form
            var customAttributesOuter = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var cartOuter = _workContext.CurrentCustomer.ShoppingCartItems
            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            .LimitPerStore(_storeContext.CurrentStore.Id)
            .ToList();
            model = _checkoutModelFactory.PrepareBillingAddressModel(cartOuter,
                selectedCountryId: model.NewAddress.CountryId,
                overrideAttributesXml: customAttributesOuter);
            return View(model);
            //return _baseNopCheckoutController.NewBillingAddress(model, form);
        }

        #endregion

        #region Confirm ActionResults

        //[ChildActionOnly]
        //public ActionResult OrderSummary(bool? prepareAndDisplayOrderReviewData)
        //{
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var model = new ShoppingCartModel();
        //    GetOrderReviewData(model, cart,
        //        isEditable: false,
        //        prepareEstimateShippingIfEnabled: false,
        //        prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
        //    return PartialView(model);
        //}

        [NonAction]
        public void UseAddressChoice(int addressId, bool shipToSameAddress = false)
        {
            var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //return RedirectToRoute("CheckoutBillingAddress");

            _workContext.CurrentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //ship to the same address?
            if (_shippingSettings.ShipToSameAddress && shipToSameAddress && cart.RequiresShipping())
            {
                _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                //reset selected shipping method (in case if "pick up in store" was selected)
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available)
                //return RedirectToRoute("CheckoutConfirm");
            }

            //return RedirectToRoute("CheckoutConfirm");
        }

        public ActionResult Confirm()
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {

                return _baseNopCheckoutController.Confirm();
            }
            return _baseNopCheckoutController.Confirm();
        }
        [HttpPost, ActionName("Confirm")]
        [ValidateInput(false)]
        public ActionResult ConfirmOrder()
        {
            var miscPlugins = _pluginFinder.GetPlugins<GBSCheckout>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                if (_workContext.CurrentCustomer.BillingAddress == null)
                {
                    Address add = new Address();
                    add.Address1 = "1912 John Towers Ave";
                    add.City = "El Cajon";
                    add.CreatedOnUtc = DateTime.Now;
                    add.Email = "info@gogbs.com";
                    _workContext.CurrentCustomer.BillingAddress = add;
                }
                var result = _baseNopCheckoutController.ConfirmOrder();

                /* Remove any uneeded sessions */
                if(typeof(RedirectToRouteResult) == result.GetType())
                _httpContext.Session.Remove("PickUpInStore");

                return result;
            }
            return _baseNopCheckoutController.ConfirmOrder();

        }
        #endregion

        #region Shopping Cart

        [ValidateInput(false)]
        [HttpPost, ActionName("ApplyDiscount")]
        [FormValueRequired("applydiscountcouponcode")]
        public ActionResult ApplyDiscount(string discountcouponcode, FormCollection form)
        {

           ActionResult applyCoupon = _baseNopShoppingCartController.ApplyDiscountCoupon(discountcouponcode, form);
        
            return Redirect(form["urlname"].ToString());
        }

        [ValidateInput(false)]
        [HttpPost, ActionName("RemoveDiscount")]
        [FormValueRequired("removesubtotaldiscount", "removeordertotaldiscount", "removediscountcouponcode")]
        public ActionResult RemoveDiscountCoupon(FormCollection form)
        {


            ActionResult removeCode = _baseNopShoppingCartController.RemoveDiscountCoupon(form);           
            return Redirect(form["urlname"].ToString());
        }
        [ValidateInput(false)]
        [HttpPost, ActionName("EstimateShippingTotal")]
        public ActionResult GetEstimateShipping(int? countryId, int? stateProvinceId, string zipPostalCode, FormCollection form)
        {

            ActionResult applyCoupon = _baseNopShoppingCartController.GetEstimateShipping(countryId, stateProvinceId, zipPostalCode, form);
            return Redirect(form["urlname"].ToString());
        }

        #endregion

        #region NonAction Functions



        [NonAction]
        protected virtual void SetBillingAddress(int? addressId, bool shipToSameAddress = false)
        {
            var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
           

            _workContext.CurrentCustomer.BillingAddress = address;
            _customerService.UpdateCustomer(_workContext.CurrentCustomer);

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //ship to the same address?
            if (_shippingSettings.ShipToSameAddress && shipToSameAddress && cart.RequiresShipping())
            {
                _workContext.CurrentCustomer.ShippingAddress = _workContext.CurrentCustomer.BillingAddress;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                //reset selected shipping method (in case if "pick up in store" was selected)
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, null, _storeContext.CurrentStore.Id);
                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available)
  
            }
        }

        [NonAction]
        protected virtual CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            //min order amount validation
            bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }
            return model;
        }


        [NonAction]
        protected virtual CheckoutPaymentInfoModel PreparePaymentInfoModel(IList<ShoppingCartItem> cart,IPaymentMethod paymentMethod)
        {
            var model = new CheckoutPaymentInfoModel();
            TempData["LocalPickup"] = false;
            /* Fill ViewBag Model */
            ViewBag.BillingAddresses = PrepareBillingAddressModel(cart, prePopulateNewAddressWithCustomerFields: true);
            ViewBag.PaymentMethod = paymentMethod.PluginDescriptor.FriendlyName.ToString();
            ViewBag.PaymentSystemName = paymentMethod.PluginDescriptor.SystemName.ToString();
            TempData["LocalPickup"] = _httpContext.Session["PickUpInStore"]; 
            
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;
            model.DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab;
            return model;
        }

        [NonAction]
        protected virtual CheckoutBillingAddressModel PrepareBillingAddressModel(IList<ShoppingCartItem> cart,
          int? selectedCountryId = null,
          bool prePopulateNewAddressWithCustomerFields = false,
          string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressModel();
            model.ShipToSameAddressAllowed = _shippingSettings.ShipToSameAddress && cart.RequiresShipping();
            model.ShipToSameAddress = true;

            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow billing
                    a.Country.AllowsBilling &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country)))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressModelFactory.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            _addressModelFactory.PrepareAddressModel(model.NewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountriesForBilling(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer,
                overrideAttributesXml: overrideAttributesXml);
            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodModel();

            var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, shippingAddress, _workContext.CurrentCustomer, "", _storeContext.CurrentStore.Id);
            
            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                                                       SystemCustomerAttributeNames.OfferedShippingOptions,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       _storeContext.CurrentStore.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                    List<DiscountForCaching> appliedDiscounts;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(
                        SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                            !String.IsNullOrEmpty(so.Name) &&
                            so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                            !String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                            so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodModel PrepareShippingMethodModel2(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodModel();

            var getShippingOptionResponse = _shippingService
                .GetShippingOptions(cart, shippingAddress, _workContext.CurrentCustomer,
                "", _storeContext.CurrentStore.Id);
            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                //                                       SystemCustomerAttributeNames.OfferedShippingOptions,
                //                                       getShippingOptionResponse.ShippingOptions,
                //                                       _storeContext.CurrentStore.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                    List<DiscountForCaching> appliedDiscounts;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(
                        SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                            !String.IsNullOrEmpty(so.Name) &&
                            so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                            !String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                            so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }

        //public ActionResult PaymentMethod()
        //{
        //    //validation
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    if (!cart.Any())
        //        return RedirectToRoute("ShoppingCart");

        //    if (_orderSettings.OnePageCheckoutEnabled)
        //        return RedirectToRoute("CheckoutOnePage");

        //    if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
        //        return new HttpUnauthorizedResult();

        //    //Check whether payment workflow is required
        //    //we ignore reward points during cart total calculation
        //    bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);
        //    if (!isPaymentWorkflowRequired)
        //    {
        //        _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
        //            SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);
        //        return RedirectToRoute("CheckoutPaymentInfo");
        //    }

        //    //filter by country
        //    int filterByCountryId = 0;
        //    if (_addressSettings.CountryEnabled &&
        //        _workContext.CurrentCustomer.BillingAddress != null &&
        //        _workContext.CurrentCustomer.BillingAddress.Country != null)
        //    {
        //        filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
        //    }

        //    //model
        //    var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);

        //    if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
        //        paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
        //    {
        //        //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
        //        //so customer doesn't have to choose a payment method

        //        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
        //            SystemCustomerAttributeNames.SelectedPaymentMethod,
        //            paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
        //            _storeContext.CurrentStore.Id);
        //        return RedirectToRoute("CheckoutPaymentInfo");
        //    }

        //    return View(paymentMethodModel);
        //}

        [NonAction]
        protected virtual CheckoutShippingAddressModel PrepareShippingAddressModel(IList<ShoppingCartItem> cart, int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
        {
            var model = new CheckoutShippingAddressModel();

            /* Set Default Address For Shipping Method */
            bool shipAddressNull = false;
            if (model.ExistingAddresses.Count > 0)
            {
                if (_workContext.CurrentCustomer.ShippingAddress == null)
                {
                    _workContext.CurrentCustomer.ShippingAddress = new Address();
                    shipAddressNull = true;
                }
                _workContext.CurrentCustomer.ShippingAddress.Address1 = (!string.IsNullOrEmpty(model.ExistingAddresses[0].Address1)) ? model.ExistingAddresses[0].Address1 : string.Empty;
                _workContext.CurrentCustomer.ShippingAddress.City = (!string.IsNullOrEmpty(model.ExistingAddresses[0].City)) ? model.ExistingAddresses[0].City : string.Empty;
                _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode = (!string.IsNullOrEmpty(model.ExistingAddresses[0].ZipPostalCode)) ? model.ExistingAddresses[0].ZipPostalCode : string.Empty;
                _workContext.CurrentCustomer.ShippingAddress.FirstName = (!string.IsNullOrEmpty(model.ExistingAddresses[0].FirstName)) ? model.ExistingAddresses[0].FirstName : string.Empty;
                _workContext.CurrentCustomer.ShippingAddress.LastName = (!string.IsNullOrEmpty(model.ExistingAddresses[0].LastName)) ? model.ExistingAddresses[0].LastName : string.Empty;
                _workContext.CurrentCustomer.ShippingAddress.Email = (!string.IsNullOrEmpty(model.ExistingAddresses[0].Email)) ? model.ExistingAddresses[0].Email : string.Empty;
                //_workContext.CurrentCustomer.ShippingAddress.Country.Name = model.ExistingAddresses[0].CountryName;
                _workContext.CurrentCustomer.ShippingAddress.PhoneNumber = (!string.IsNullOrEmpty(model.ExistingAddresses[0].PhoneNumber)) ? model.ExistingAddresses[0].PhoneNumber : string.Empty;
            }
            if (shipAddressNull)
            {
                ViewBag.ShippingMethod = PrepareShippingMethodModel2(cart, _workContext.CurrentCustomer.ShippingAddress);
            }
            else
            {
                //if (!string.IsNullOrEmpty(HttpContext.Request.QueryString["addressid"]))
                //{
                //    int addressId = Int32.Parse(HttpContext.Request.QueryString["addressid"]);
                //    TempData["ShippingAddressId"] = addressId;
                //    _baseNopCheckoutController.SelectShippingAddress(addressId);

                //}
                
                //ViewBag.ShippingMethod = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            }

            //allow pickup in store?
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
            if (model.AllowPickUpInStore)
            {
                model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
                model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
                var pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, _workContext.CurrentCustomer, null, _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            var country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            var pickupPointModel = new CheckoutPickupPointModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City,
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                var amount = _taxService.GetShippingPrice(x.PickupFee, _workContext.CurrentCustomer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, _workContext.WorkingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }

                            return pickupPointModel;
                        }).ToList();
                    else
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Warnings.Add(error);
                }

                //only available pickup points
                if (!_shippingService.LoadActiveShippingRateComputationMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id).Any())
                {
                    if (!pickupPointProviders.Any())
                    {
                        model.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                        model.Warnings.Add(_localizationService.GetResource("Checkout.PickupPoints.NotAvailable"));
                    }
                    model.PickUpInStoreOnly = true;
                    model.PickUpInStore = true;
                    return model;
                }
            }

            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow shipping
                    a.Country.AllowsShipping &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country)))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressModelFactory.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }


            if (!string.IsNullOrEmpty(HttpContext.Request.QueryString["addressid"]))
            {
                int addressId = Int32.Parse(HttpContext.Request.QueryString["addressid"]);
                TempData["ShippingAddressId"] = addressId;
                _baseNopCheckoutController.SelectShippingAddress(addressId);

            }else if(TempData.Peek("ShippingAddressId") != null && TempData.Peek("ShippingAddressId").ToString() == "LocalPickup")
            {
                //all good
            }
            else if (TempData.Peek("ShippingAddressId") != null && Int32.Parse(TempData.Peek("ShippingAddressId").ToString()) > 0)
            {
                _baseNopCheckoutController.SelectShippingAddress(Int32.Parse(TempData.Peek("ShippingAddressId").ToString()));
            }
            else
            {
                if (addresses.Count > 0)
                {
                    TempData["ShippingAddressId"] = addresses[0].Id;
                    _baseNopCheckoutController.SelectShippingAddress(addresses[0].Id);
                }
            }

            ViewBag.ShippingMethod = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);

            
            //new address
            model.NewAddress.CountryId = selectedCountryId;
            _addressModelFactory.PrepareAddressModel(model.NewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer,
                overrideAttributesXml: overrideAttributesXml);

            

            return model;
        }

        [NonAction]
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, ignoreRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }
        [NonAction]
        protected void GetOrderReviewData(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                        _addressModelFactory.PrepareAddressModel(model.OrderReviewData.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);

                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    var pickupPoint = _workContext.CurrentCustomer
                        .GetAttribute<PickupPoint>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                    model.OrderReviewData.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        if (_workContext.CurrentCustomer.ShippingAddress != null)
                        {
                                _addressModelFactory.PrepareAddressModel(model.OrderReviewData.ShippingAddress,
                                address: _workContext.CurrentCustomer.ShippingAddress,
                                excludeProperties: false,
                                addressSettings: _addressSettings);
                        }
                    }
                    else
                    {
                        var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                        model.OrderReviewData.PickupAddress = new AddressModel
                        {
                            Address1 = pickupPoint.Address,
                            City = pickupPoint.City,
                            CountryName = country != null ? country.Name : string.Empty,
                            ZipPostalCode = pickupPoint.ZipPostalCode
                        };
                    }

                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";
                //custom values
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest != null)
                {
                    model.OrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }
        #endregion NonAction Functions
    }
}