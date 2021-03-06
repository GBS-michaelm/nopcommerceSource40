using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Services.Orders;
using WebServices.Models.Orders;
using GBSOrderIDConvert;
using Nop.Plugin.Order.GBS;
using Nop.Data;
using Nop.Core.Data;
using Nop.Plugin.DataAccess.GBS;
using Nop.Web.Framework;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas;
using Nop.Services.Configuration;
//using WebServices.Models.File;
using Newtonsoft.Json;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Nop.Plugin.Order.GBS.Models;
using WebServices.Models.File;

namespace Nop.Services.Custom.Orders
{
    public class GBSOrderProcessingService : OrderProcessingService
    {
        private readonly GBSOrderSettings _gbsOrderSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;        //private readonly GBSOrderSettings _gbsStoreNamePrepend;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly CcSettings _customersCanvasSettings;
        private readonly ILogger _logger;
        private readonly IProductAttributeFormatter  _productAttributeFormatter;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ICustomerService _customerService;
        DBManager manager = new DBManager();
        ICcService ccService = EngineContext.Current.Resolve<ICcService>();

        public GBSOrderProcessingService(
            ISettingService settingService,
            IStoreContext storeContext,
            IPluginFinder pluginFinder, 
            GBSOrderSettings gbsOrderSettings, 
            IOrderService orderService, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            ILanguageService languageService, 
            IProductService productService, 
            IPaymentService paymentService, 
            ILogger logger, 
            IOrderTotalCalculationService orderTotalCalculationService, 
            IPriceCalculationService priceCalculationService, 
            IPriceFormatter priceFormatter, 
            IProductAttributeParser productAttributeParser, 
            IProductAttributeFormatter productAttributeFormatter, 
            IGiftCardService giftCardService, 
            IShoppingCartService shoppingCartService, 
            ICheckoutAttributeFormatter checkoutAttributeFormatter, 
            IShippingService shippingService, 
            IShipmentService shipmentService, 
            ITaxService taxService, 
            ICustomerService customerService, 
            IDiscountService discountService, 
            IEncryptionService encryptionService, 
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService, 
            IVendorService vendorService, 
            ICustomerActivityService customerActivityService, 
            ICurrencyService currencyService, 
            IAffiliateService affiliateService,
            IEventPublisher eventPublisher, 
            IPdfService pdfService, 
            IRewardPointService rewardPointService,
            IGenericAttributeService genericAttributeService, 
            ShippingSettings shippingSettings, 
            PaymentSettings paymentSettings, 
            RewardPointsSettings rewardPointsSettings, 
            OrderSettings orderSettings, 
            TaxSettings taxSettings, 
            LocalizationSettings localizationSettings,
            CurrencySettings currencySettings,
            ICountryService countryService,
            IStateProvinceService stateProviceService,
            ICustomNumberFormatter customNumberFormatter,
			IHttpContextAccessor httpContext) 
            : base
            (orderService, 
                  webHelper, 
                  localizationService, 
                  languageService, 
                  productService, 
                  paymentService, 
                  logger,
                  orderTotalCalculationService, 
                  priceCalculationService, 
                  priceFormatter,
                  productAttributeParser, 
                  productAttributeFormatter,
                  giftCardService, 
                  shoppingCartService,
                  checkoutAttributeFormatter,
                  shippingService, 
                  shipmentService, 
                  taxService,
                  customerService,
                  discountService,
                  encryptionService, 
                  workContext,
                  workflowMessageService, 
                  vendorService,
                  customerActivityService,
                  currencyService,
                  affiliateService, 
                  eventPublisher, 
                  pdfService, 
                  rewardPointService, 
                  genericAttributeService,
                  countryService,
                  stateProviceService,
                  shippingSettings, 
                  paymentSettings,
                  rewardPointsSettings,
                  orderSettings,
                  taxSettings, 
                  localizationSettings, 
                  currencySettings,
                  customNumberFormatter
                  )
        {

            this._gbsOrderSettings = gbsOrderSettings;
            this._pluginFinder = pluginFinder;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productAttributeParser = productAttributeParser;
            this._customersCanvasSettings = settingService.LoadSetting<CcSettings>();
            this._logger = logger;
            this._productAttributeFormatter = productAttributeFormatter;
            this._httpContext = httpContext;
            this._taxService = taxService;
            this._priceCalculationService = priceCalculationService;
            this._customerService = customerService;
        }

        private void SaveGBSOrderID(string gbsOrderId, int NOPOrderID)
        {
            //write code to save this to our new table.
        }
        public override PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest)
        {
            PlaceOrderResult myResult = null;
            CustomTokenProvider orderProv = null;
            string gbsOrderId = null;


            var customer = _workContext.CurrentCustomer;
            try
            {

                var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: processPaymentRequest.StoreId).ToList();
                if (miscPlugins.Count > 0) {

                    //processPaymentRequest.CustomValues.Clear();

                    string address = _gbsOrderSettings.GBSOrderWebServiceAddress;
                    GBSOrderServiceClient myOrderService = new GBSOrderServiceClient();

                    OrderIDConversionModel orderData = new OrderIDConversionModel(); //object that will hold the website name inside of it productData.websiteName
                    orderData.websiteName = _gbsOrderSettings.GBSStoreNamePrepend; //"NOP";
                    gbsOrderId = myOrderService.ConvertID(orderData, address, _gbsOrderSettings.LoginId, _gbsOrderSettings.Password);
                    Object value = null;

					NopResourceDisplayNameAttribute orderNumberKeyGBS = new NopResourceDisplayNameAttribute(("Account.CustomerOrders.OrderNumber"));

                    // will be return from calling 
                    if (processPaymentRequest.CustomValues.TryGetValue(orderNumberKeyGBS.DisplayName, out value))
                    {
                        processPaymentRequest.CustomValues[orderNumberKeyGBS.DisplayName] = gbsOrderId;
                    }
                    else
                    {
                        processPaymentRequest.CustomValues.Add(orderNumberKeyGBS.DisplayName, gbsOrderId);
                    }

                    string addContactNum = _httpContext.HttpContext.Session.GetString("customerPhoneNumber") == null ? "" : _httpContext.HttpContext.Session.GetString("customerPhoneNumber").ToString();
                    if (addContactNum != null && addContactNum != "")
                    {
                        processPaymentRequest.CustomValues["Pickup Contact Phone"] = addContactNum;
                        //processPaymentRequest.CustomValues.Add("Pickup Contact Phone", addContactNum);
                        _httpContext.HttpContext.Session.Remove("customerPhoneNumber");
                    }


                    if (processPaymentRequest.PaymentMethodSystemName == "Payments.GBS.PurchaseOrder")
                    {
                        string POnum = _httpContext.HttpContext.Session.GetString("purchaseOrderNumber") == null ? "" : _httpContext.HttpContext.Session.GetString("purchaseOrderNumber").ToString();
                        if (POnum != null && POnum != "")
                        {
                            processPaymentRequest.CustomValues["PO Number"] = POnum;

                            //processPaymentRequest.CustomValues.Add("PO Number", POnum);
                            _httpContext.HttpContext.Session.Remove("purchaseOrderNumber");
                        }

                        string POname = _httpContext.HttpContext.Session.GetString("purchaseOrderName") == null ? "" : _httpContext.HttpContext.Session.GetString("purchaseOrderName").ToString();
                        if (POname != null && POname != "")
                        {
                            processPaymentRequest.CustomValues["PO Name"] = POname;

                            //processPaymentRequest.CustomValues.Add("PO Name", POname);
                            _httpContext.HttpContext.Session.Remove("purchaseOrderName");
                        }

                        string POphone = _httpContext.HttpContext.Session.GetString("purchaseOrderPhoneNumber") == null ? "" : _httpContext.HttpContext.Session.GetString("purchaseOrderPhoneNumber").ToString();
                        if (POphone != null && POphone != "")
                        {
                            processPaymentRequest.CustomValues["PO Phone"] = POphone;

                            //processPaymentRequest.CustomValues.Add("PO Phone", POphone);
                            _httpContext.HttpContext.Session.Remove("purchaseOrderPhoneNumber");
                        }
                    }

                    if (processPaymentRequest.PaymentMethodSystemName == "Payments.GBS.MonthlyBilling")
                    {
                        string MBname = _httpContext.HttpContext.Session.GetString("monthlyBillingName") == null ? "" : _httpContext.HttpContext.Session.GetString("monthlyBillingName").ToString();
                        if (MBname != null && MBname != "")
                        {
                            processPaymentRequest.CustomValues["Monthly Billing Name"] = MBname;

                            //processPaymentRequest.CustomValues.Add("Monthly Billing Name", MBname);
                            _httpContext.HttpContext.Session.Remove("monthlyBillingName");
                        }

                        string MBphone = _httpContext.HttpContext.Session.GetString("monthlyBillingPhoneNumber") == null ? "" : _httpContext.HttpContext.Session.GetString("monthlyBillingPhoneNumber").ToString();
                        if (MBphone != null && MBphone != "")
                        {
                            processPaymentRequest.CustomValues["Monthly Billing Phone"] = MBphone;

                            //processPaymentRequest.CustomValues.Add("Monthly Billing Phone", MBphone);
                            _httpContext.HttpContext.Session.Remove("monthlyBillingPhoneNumber");
                        }

                        string MBref = _httpContext.HttpContext.Session.GetString("monthlyBillingReference") == null ? "" : _httpContext.HttpContext.Session.GetString("monthlyBillingReference").ToString();
                        if (MBref != null && MBref != "")
                        {
                            //MBref = "<![CDATA[" + MBref + "]]>";
                            MBref = MBref.Replace("&", " ");
                            MBref = MBref.Replace("<", " ");
                            MBref = MBref.Replace(">", " ");
                            MBref = MBref.Replace("\"", " ");
                            MBref = MBref.Replace("\'", " ");
                            processPaymentRequest.CustomValues["Monthly Billing Reference"] = MBref;

                            //processPaymentRequest.CustomValues.Add("Monthly Billing Reference", MBref);
                            _httpContext.HttpContext.Session.Remove("monthlyBillingReference");
                        }
                    }


                }


                myResult = base.PlaceOrder(processPaymentRequest);

                if (miscPlugins.Count > 0)
                {
                    


                    if (myResult.PlacedOrder != null)
                    {

                        string insert = "";
                        
                        List<ExtendedOrderItem> extendedOrderItems = new List<ExtendedOrderItem>();
                        List<ProductFileModel> ccFiles = new List<ProductFileModel>();

                        foreach (var item in myResult.PlacedOrder.OrderItems)
                        {
                            ExtendedOrderItem extendedOrderItem = new ExtendedOrderItem();
                            extendedOrderItem.OrderItemID = item.Id;
                            bool isCCProduct = ccService.IsProductForCc(item.ProductId);
                            if (isCCProduct)
                            {
                                ProcessCanvasProducts(item, extendedOrderItem, ccFiles, false);
                            }
                            extendedOrderItems.Add(extendedOrderItem);
                        }

                        //save to tblNopOrder
                        SaveExtendedOrdertItems(extendedOrderItems);


                        //call file service to save files
                        SaveProductionFiles(ccFiles, gbsOrderId,false);


                        //Make saving to tblNopOrder last operation so that migration only pulls data after all other dependendent data is committed.
                        Dictionary<string, Object> paramDic = new Dictionary<string, Object>();
                        paramDic.Add("@nopID", myResult.PlacedOrder.Id.ToString());
                        paramDic.Add("@gbsOrderID", gbsOrderId);
                        insert = "EXEC Insert_tblNOPOrder @nopID,@gbsOrderID";
                        //add impersonation if exists
                        if (_workContext.OriginalCustomerIfImpersonated != null)
                        {
                            paramDic.Add("@impersonator", _workContext.OriginalCustomerIfImpersonated.Id);
                            insert = "EXEC Insert_tblNOPOrder @nopID,@gbsOrderID,@impersonator";

                        }
                        manager.SetParameterizedQueryNoData(insert, paramDic);

                        _httpContext.HttpContext.Session.Remove("customerPhoneNumber");
                        _httpContext.HttpContext.Session.Remove("purchaseOrderNumber");
                        _httpContext.HttpContext.Session.Remove("purchaseOrderName");
                        _httpContext.HttpContext.Session.Remove("purchaseOrderPhoneNumber");
                        _httpContext.HttpContext.Session.Remove("monthlyBillingName");
                        _httpContext.HttpContext.Session.Remove("monthlyBillingPhoneNumber");
                        _httpContext.HttpContext.Session.Remove("monthlyBillingReference");

                    }//if (myResult.PlacedOrder != null)
                    else
                    {
						PlaceOrderContainer orderContainer = this.PreparePlaceOrderDetails(processPaymentRequest);
                        SaveFailedOrder(processPaymentRequest, orderContainer, myResult); 
                    }
                }          


            }
            catch (Exception ex)
            {
                _logger.Error("Error in Order Service", ex, customer);
				PlaceOrderContainer orderContainer = this.PreparePlaceOrderDetails(processPaymentRequest);
                SaveFailedOrder(processPaymentRequest, orderContainer, myResult, ex);
                throw ex;
            }

            return myResult;

        }

        public int getCCIdFromItem(string xml)
        {
            var customer = _workContext.CurrentCustomer;

            var mappings = _productAttributeParser.ParseProductAttributeMappings(xml);
            var mapping = mappings.FirstOrDefault(x => x.ProductAttributeId == _customersCanvasSettings.CcIdAttributeId);
            if (mapping == null)
                _logger.Error("Custom product does not have a ccID", null, customer);

            var values = _productAttributeParser.ParseValues(xml, mapping.Id);
            if (values == null || !values.Any())
                _logger.Error("Custom product ccID does not have a value", null, customer);



            #region NoteCard Processing
            //old region. put in ProcessNotecards
            #endregion NoteCard Processing

            var designId = Convert.ToInt32(values.First());
            return designId;
        }

        public void ProcessCanvasProducts(OrderItem item, ExtendedOrderItem extendedOrderItem, List<ProductFileModel> ccFiles, bool failedOrder)
        {
            var customer = _workContext.CurrentCustomer;
            var ccResult = ccService.GetCcResult(item.AttributesXml);

            var designId = getCCIdFromItem(item.AttributesXml);
            var design = ccService.GetDesign(designId);
            dynamic designData = JsonConvert.DeserializeObject<Object>(design.Data);
            extendedOrderItem.ccID = designId;
            foreach (dynamic data in designData)
            {
                string dataName = (string)data.Name;
                string stateID = (string)data;
                foreach (string cc in ccResult.HiResUrls)
                {
                    if (cc.Contains(stateID))
                    {
                        ProductFileModel ccProduct = new ProductFileModel();
                        ccProduct.product.productCode = item.Product.Sku;
                        var index = cc.Split('/').Last<string>().Split('_').First<string>();
                        string productType = dataName.Split('-').First<string>().ToLower();
                        ccProduct.product.productType = productType + "-" + index;
                        ccProduct.product.hiResPDFURL = cc;

                        ccProduct.product.sourceReference = item.Id.ToString();
                        if (failedOrder)
                        {
                            ccProduct.product.sourceReference = extendedOrderItem.ccID.ToString();
                        }
                        ccProduct.requestSessionID = _httpContext.HttpContext.Session.Id;

                        #region NoteCard Processing
                        ProcessNotecards(ccFiles, ccProduct, productType, item);
                        #endregion NoteCard Processing

                    }
                }

            }

        }


        public void SaveProductionFiles (List<ProductFileModel> ccFiles, string gbsOrderID, bool failedOrder)
        {
            var customer = _workContext.CurrentCustomer;

            string fileServiceaddress = _gbsOrderSettings.GBSPrintFileWebServiceAddress;
            GBSFileService.GBSFileServiceClient FileService = new GBSFileService.GBSFileServiceClient();
            if (ccFiles.Count > 0)
            {
                List<List<ProductFileModel>> chunksOf3 = SplitList(ccFiles, 3);
                foreach (var ccFilesOf3 in chunksOf3)
                {
                    try
                    {
                        string response = FileService.CopyFilesToProduction(ccFilesOf3, fileServiceaddress, _gbsOrderSettings.LoginId, _gbsOrderSettings.Password);
                        List<ProductFileModel> responseFiles = JsonConvert.DeserializeObject<List<ProductFileModel>>(response);
                        foreach (ProductFileModel product in responseFiles)
                        {
                            if (!String.IsNullOrEmpty(product.product.productionFileName) && !product.product.productionFileName.ToLower().Contains("exception"))
                            {
                                Dictionary<string, string> paramDicEx = new Dictionary<string, string>();
                                if (failedOrder)
                                {
                                    paramDicEx.Add("@nopOrderItemID", Convert.ToString(0));
                                    paramDicEx.Add("@ccID", product.product.sourceReference);
                                }
                                else
                                {
                                    paramDicEx.Add("@nopOrderItemID", product.product.sourceReference);
                                    paramDicEx.Add("@ccID", "");
                                }
                                paramDicEx.Add("@ProductType", product.product.productType);
                                paramDicEx.Add("@FileName", product.product.productionFileName);
                                paramDicEx.Add("@gbsOrderID", gbsOrderID);


                                //insert = "INSERT INTO tblNOPProductionFiles (nopOrderItemID, ProductType,FileName) ";
                                //insert += "VALUES ('" + product.product.sourceReference + "', '" + product.product.productType + "', '" + product.product.productionFileName + "')";
                                var insert = "EXEC Insert_tblNOPProductionFiles @nopOrderItemID,@ProductType,@FileName,@gbsOrderID,@ccID";
                                manager.SetParameterizedQueryNoData(insert, paramDicEx);
                            }
                            else
                            {
                                _logger.Error("Error with product filename" + response, null, customer);
                            }
                        }

                    }
                    catch (Exception eee)
                    {
                        _logger.Error("Error accesing File Service", eee, customer);
                    }
                }
            }

        }

        public void SaveExtendedOrdertItems (List<ExtendedOrderItem> items)
        {

            foreach (var item in items)
            {

                Dictionary<string, string> paramDicEx = new Dictionary<string, string>();
                paramDicEx.Add("@nopOrderItemID", item.OrderItemID.ToString());
                paramDicEx.Add("@ccID", item.ccID.ToString());

                var insert = "EXEC Insert_tblNOPOrderItem @nopOrderItemID,@ccID";
                manager.SetParameterizedQueryNoData(insert, paramDicEx);

            }
        }

        public void ProcessNotecards(List<ProductFileModel> ccFiles, ProductFileModel ccProduct, string productType, OrderItem item)
        {
            var customer = _workContext.CurrentCustomer;

            var selectedGreetingOption = GetGreetingOption(item);
            var selectedGreetingOrientation = "H";
            if (selectedGreetingOption.ToLower() == "yes")
            {
                selectedGreetingOrientation = GetGreetingOrientation(item);
            }
            else if (selectedGreetingOption.ToLower() == "not found")
            {
                _logger.Error("Greeting Option Selection not found for item: " + item.Product.Sku + "and orderid: " + item.OrderId, null, customer);
            }

            var selectedReturnAddressOption = GetReturnAddressEnvelopeOption(item);
            var selectedReturnAddressSide = "F";
            if (selectedReturnAddressOption.ToLower() == "yes")
            {
                selectedReturnAddressSide = GetReturnAddressEnvelopeSide(item);
            }
            else if (selectedReturnAddressOption.ToLower() == "not found")
            {
                _logger.Error("Return Address Option Selection not found for item: " + item.Product.Sku + "and orderid: " + item.OrderId, null, customer);
            }

            var selectedCoverOrientation = GetCoverOrientation(item);
            var selectedBackOrientation = GetBackOrientation(item);
            bool add = true;
            #region Eliminate unneeded print files
            if (ccProduct.product.productType.ToLower() == "notecard-1" && selectedGreetingOption.ToLower() == "no")
            {
                //customer did not select a custom greeting so don't add the print file
                add = false;
            }
            if (productType == "envelope" && selectedReturnAddressOption.ToLower() == "no")
            {
                //customer did not select a custom envelope so don't add the print file
                add = false;
            }
            else if (productType == "envelope")
            {
                //if they did select a custom envelope, then add only the side they selected.
                if ((selectedReturnAddressSide == "F" && ccProduct.product.productType.ToLower() == "envelope-1") || (selectedReturnAddressSide == "B" && ccProduct.product.productType.ToLower() == "envelope-0"))
                {
                    add = false;
                }
            }

            #endregion Eliminate unneeded print files

            #region   Reset product types, orientation, color model and surface selections

            if (ccProduct.product.productType.ToLower() == "notecard-0")
            {
                //Card Front
                ccProduct.product.orientation = selectedCoverOrientation;
                ccProduct.product.productType = "notecard";
                ccProduct.product.surface = "front";
            }
            if (ccProduct.product.productType.ToLower() == "notecard-1")
            {
                //Card Greeting
                ccProduct.product.orientation = selectedGreetingOrientation;
                ccProduct.product.productType = "notecard";
                ccProduct.product.surface = "greeting";
                ccProduct.product.imprintColor = "1";
            }
            if (ccProduct.product.productType.ToLower() == "notecard-2")
            {
                //Card Back
                ccProduct.product.orientation = selectedBackOrientation;
                ccProduct.product.productType = "notecard";
                ccProduct.product.surface = "back";
            }
            if (productType == "envelope")
            {
                ccProduct.product.productType = "envelope";
                ccProduct.product.surface = selectedReturnAddressSide;
                ccProduct.product.imprintColor = "1";
            }
            #endregion Reset product types, orientation and surface selections
            if (add)
            {
                ccFiles.Add(ccProduct);
            }
        }

        public static List<List<T>> SplitList<T>(List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }

        public string GetReturnAddressEnvelopeOption(OrderItem item)
        {
            try
            {
                var mappings = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                IList<string> pValues = null;
                foreach (ProductAttributeMapping pMapping in mappings)
                {

                    if (pMapping.ProductAttribute.Name != null && pMapping.ProductAttribute.Name.ToLower() == "add return address")
                    {
                        pValues = _productAttributeParser.ParseValues(item.AttributesXml, pMapping.Id);
                        break;
                    }
                }
                var paValues = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml);
                foreach (ProductAttributeValue attr in paValues)
                {
                    if (attr.Id == Int32.Parse(pValues[0]))
                    {
                        return attr.Name;
                    }
                }
                return "not found";
            }catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Return Address Option Value for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);
                return "not found";
            }
        }
        public string GetReturnAddressEnvelopeSide(OrderItem item)
        {
            try
            {
                var mappings = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                IList<string> pValues = null;
                foreach (ProductAttributeMapping pMapping in mappings)
                {

                    if (pMapping.ProductAttribute.Name != null && pMapping.ProductAttribute.Name.ToLower() == "return address placement")
                    {
                        pValues = _productAttributeParser.ParseValues(item.AttributesXml, pMapping.Id);
                        if (pValues[0].ToLower() == "back side")
                        {
                            return "B";
                        }
                        else
                        {
                            return "F";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Return Address Side Value for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);
            }
            return "not found";
        }

        public string GetCoverOrientation(OrderItem item)
        {
            try
            {
                ICollection<ProductSpecificationAttribute> specifications = item.Product.ProductSpecificationAttributes;
                foreach (ProductSpecificationAttribute attr in specifications)
                {
                    if (attr.SpecificationAttributeOption.SpecificationAttribute.Name.ToLower() == "orientation")
                    {
                        if (attr.SpecificationAttributeOption.Name.ToLower() == "vertical")
                        {
                            return "V";
                        }
                        else
                        {
                            return "H";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Card Front Orientation for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);

            }
            return "H";
        }

        public string GetGreetingOrientation(OrderItem item)
        {
            try
            {
                ICollection<ProductSpecificationAttribute> specifications = item.Product.ProductSpecificationAttributes;
                foreach (ProductSpecificationAttribute attr in specifications)
                {
                    if (attr.SpecificationAttributeOption.SpecificationAttribute.Name.ToLower() == "orientation")
                    {
                        if (attr.SpecificationAttributeOption.Name.ToLower() == "vertical")
                        {
                            return "V";
                        } else 
                        {
                            return "H";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Card Greeting Orientation for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);

            }
            return "H";
        }

        public string GetBackOrientation(OrderItem item)
        {
            try
            {
                ICollection<ProductSpecificationAttribute> specifications = item.Product.ProductSpecificationAttributes;
                foreach (ProductSpecificationAttribute attr in specifications)
                {
                    if (attr.SpecificationAttributeOption.SpecificationAttribute.Name.ToLower() == "orientation")
                    {
                        if (attr.SpecificationAttributeOption.Name.ToLower() == "vertical")
                        {
                            return "V";
                        }
                        else
                        {
                            return "H";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Card Back Orientation for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);

            }
            return "H";
        }

        public string GetGreetingOption(OrderItem item)
        {
            try
            {
                var mappings = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                IList<string> pValues = null;
                foreach (ProductAttributeMapping pMapping in mappings)
                {

                    if (pMapping.ProductAttribute.Name != null && pMapping.ProductAttribute.Name.ToLower() == "add greeting")
                    {
                        pValues = _productAttributeParser.ParseValues(item.AttributesXml, pMapping.Id);
                        break;
                    }
                }
                var paValues = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml);
                foreach (ProductAttributeValue attr in paValues)
                {
                    if (attr.Id == Int32.Parse(pValues[0]))
                    {
                        return attr.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error getting Greeting Opton Value for item: " + item.Product.Sku + "and orderid: " + item.OrderId, ex, customer);
            }
            return "not found";
        }

        //GBS Custom Attribute Update
        protected override void SendNotificationsAndSaveNotes(Nop.Core.Domain.Orders.Order order)
        {

            Dictionary<string, Object> NotecardSetQty = GetNoteCardSetCount(order);
            foreach (var item in order.OrderItems)
            {
                if (NotecardSetQty.ContainsKey(item.ProductId.ToString()))
                {
                    item.AttributeDescription += "<br/>" + NotecardSetQty[item.ProductId.ToString()];
                }

            }
            base.SendNotificationsAndSaveNotes(order);
        }

        public Dictionary<string, object> GetNoteCardSetCount(Nop.Core.Domain.Orders.Order order)
        {
            var spec = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            Dictionary<string, object> productSpecsDict = new Dictionary<string, object>();
            var customer = _workContext.CurrentCustomer;


            foreach (var item in order.OrderItems)
            {
                var specAttr = spec.GetProductSpecificationAttributes(item.ProductId);
                var option = specAttr.Select(x => x.SpecificationAttributeOption);
                var getOption = option.Where(x => x.SpecificationAttribute.DisplayOrder == 9999);
                if (getOption.Count<SpecificationAttributeOption>() > 0)
                {
                    if (!productSpecsDict.ContainsKey(item.ProductId.ToString()))
                        productSpecsDict.Add(item.ProductId.ToString(), getOption.FirstOrDefault().Name);

                }
            }

            return productSpecsDict;
        }
        protected void SaveFailedOrder(ProcessPaymentRequest paymentRequest, PlaceOrderContainer orderContainer, PlaceOrderResult placeOrderResult, Exception theException = null)
        {
			NopResourceDisplayNameAttribute orderNumberKeyGBS = new NopResourceDisplayNameAttribute(("Account.CustomerOrders.OrderNumber"));
            string GBSOrderID = (string)paymentRequest.CustomValues[orderNumberKeyGBS.DisplayName];

            string insert = "";
            try
            {
                int StoreID = paymentRequest.StoreId;
                int CustomerId = paymentRequest.CustomerId;
                int BillingAddressId = orderContainer.Customer.BillingAddress != null ? orderContainer.Customer.BillingAddress.Id : 0;
                int ShippingAddressId = orderContainer.Customer.ShippingAddress != null ?  orderContainer.Customer.ShippingAddress.Id : 0;
                int PickupAddressId = orderContainer.PickupAddress != null ?  orderContainer.PickupAddress.Id : 0;
                bool PickUpInStore = orderContainer.PickUpInStore;
                string PaymentMethodSystemName = paymentRequest.PaymentMethodSystemName != null ? paymentRequest.PaymentMethodSystemName : String.Empty;
                string CustomerCurrencyCode = orderContainer.CustomerCurrencyCode != null ? orderContainer.CustomerCurrencyCode : String.Empty;
                decimal CurrencyRate = orderContainer.CustomerCurrencyRate;
                int CustomerTaxDisplayTypeId = (int)orderContainer.CustomerTaxDisplayType;
                string VatNumber = orderContainer.VatNumber != null ? orderContainer.VatNumber : String.Empty;
                decimal OrderSubtotalInclTax = orderContainer.OrderSubTotalInclTax;
                decimal OrderSubtotalExclTax = orderContainer.OrderSubTotalExclTax;
                decimal OrderSubTotalDiscountInclTax = orderContainer.OrderSubTotalDiscountInclTax;
                decimal OrderSubTotalDiscountExclTax = orderContainer.OrderSubTotalDiscountExclTax;
                decimal OrderShippingInclTax = orderContainer.OrderShippingTotalInclTax;
                decimal OrderShippingExclTax = orderContainer.OrderShippingTotalExclTax;
                decimal PaymentMethodAdditionalFeeInclTax = 0;
                decimal PaymentMethodAdditionalFeeExclTax = 0;
                string TaxRates = orderContainer.TaxRates != null ? orderContainer.TaxRates : String.Empty;
                decimal OrderTax = orderContainer.OrderTaxTotal;
                decimal OrderDiscount = orderContainer.OrderDiscountAmount;
                decimal OrderTotal = orderContainer.OrderTotal;
                bool RewardPointsWereAdded = (orderContainer.RedeemedRewardPoints > 0);
                string CheckoutAttributeDescription = orderContainer.CheckoutAttributeDescription != null ? orderContainer.CheckoutAttributeDescription : String.Empty;
                string CheckoutAttributesXml = orderContainer.CheckoutAttributesXml != null ? orderContainer.CheckoutAttributesXml : String.Empty;
                int CustomerLanguageId = orderContainer.CustomerLanguage != null ? orderContainer.CustomerLanguage.Id : 0;
                int AffiliateId = orderContainer.AffiliateId;
				string CustomerIp = _httpContext.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString(); //_httpContext.HttpContext.Request.Host != null ? HttpContext.Current.Request.UserHostAddress : String.Empty;
                string CardType = paymentRequest.CreditCardType != null ? paymentRequest.CreditCardType : String.Empty;
                string CardName = paymentRequest.CreditCardName != null ? paymentRequest.CreditCardName : String.Empty;
                string MaskedCreditCardNumber = String.Empty;
                try
                {
                    MaskedCreditCardNumber = !String.IsNullOrEmpty(paymentRequest.CreditCardNumber) ? string.Format("************{0}", paymentRequest.CreditCardNumber.Trim().Substring(12, 4)) : String.Empty;
                }
                catch (Exception ex1) { }
                string CardExpirationMonth = paymentRequest.CreditCardExpireMonth.ToString();
                string CardExpirationYear = paymentRequest.CreditCardExpireYear.ToString();
                string ShippingMethod = orderContainer.ShippingMethodName != null ? orderContainer.ShippingMethodName : String.Empty;
                string ShippingRateComputationMethodSystemName = orderContainer.ShippingRateComputationMethodSystemName != null ? orderContainer.ShippingRateComputationMethodSystemName : String.Empty;
                string CustomValuesXml = "<CustomValues>";
                foreach(KeyValuePair<string, object> entry in paymentRequest.CustomValues)
                {
                    CustomValuesXml += "<"+ entry.Key != null ? entry.Key : String.Empty +">" + entry.Value != null ? entry.Value.ToString() : String.Empty + "</" + entry.Key != null ? entry.Key : String.Empty + ">";
                }
                CustomValuesXml += "</CustomValues>";

                if (String.IsNullOrEmpty(CardType))
                {
                    string firstDigit = paymentRequest.CreditCardNumber;
                    firstDigit = !String.IsNullOrEmpty(firstDigit) ? firstDigit.Substring(0, firstDigit.Length - (firstDigit.Length - 1)) : "U";
                    switch (firstDigit)
                    {
                        case "4":
                            CardType = "V";
                            break;
                        case "5":
                            CardType = "M";
                            break;
                        case "3":
                            CardType = "A";
                            break;
                        case "6":
                            CardType = "D";
                            break;
                        default:
                            CardType = "Unknown";
                            break;
                    }
                }
                String errors = "";
                foreach (string error in placeOrderResult.Errors)
                {
                    errors += ": "+error;
                }
                if (theException != null)
                {
                    errors += "Exception Message: " + theException.Message + " Exception Stacktrace: " + theException.StackTrace;
                }

                DBManager manager = new DBManager();
                Dictionary<string, Object> paramDic = new Dictionary<string, Object>();
                paramDic.Add("@GBSOrderID", GBSOrderID);
                paramDic.Add("@StoreID", StoreID);
                paramDic.Add("@CustomerId", CustomerId);
                paramDic.Add("@BillingAddressId", BillingAddressId);
                paramDic.Add("@ShippingAddressId", ShippingAddressId);
                paramDic.Add("@PickupAddressId", PickupAddressId);
                paramDic.Add("@PickUpInStore", PickUpInStore);
                paramDic.Add("@PaymentMethodSystemName", PaymentMethodSystemName);
                paramDic.Add("@CustomerCurrencyCode", CustomerCurrencyCode);
                paramDic.Add("@CurrencyRate", CurrencyRate);
                paramDic.Add("@CustomerTaxDisplayTypeId", CustomerTaxDisplayTypeId);
                paramDic.Add("@VatNumber", VatNumber);
                paramDic.Add("@OrderSubtotalInclTax", OrderSubtotalInclTax);
                paramDic.Add("@OrderSubtotalExclTax", OrderSubtotalExclTax);
                paramDic.Add("@OrderSubTotalDiscountInclTax", OrderSubTotalDiscountInclTax);
                paramDic.Add("@OrderSubTotalDiscountExclTax", OrderSubTotalDiscountExclTax);
                paramDic.Add("@OrderShippingInclTax", OrderShippingInclTax);
                paramDic.Add("@OrderShippingExclTax", OrderShippingExclTax);
                paramDic.Add("@PaymentMethodAdditionalFeeInclTax", PaymentMethodAdditionalFeeInclTax);
                paramDic.Add("@PaymentMethodAdditionalFeeExclTax", PaymentMethodAdditionalFeeExclTax);
                paramDic.Add("@TaxRates", TaxRates);
                paramDic.Add("@OrderTax", OrderTax);
                paramDic.Add("@OrderDiscount", OrderDiscount);
                paramDic.Add("@OrderTotal", OrderTotal);
                paramDic.Add("@RewardPointsWereAdded", RewardPointsWereAdded);
                paramDic.Add("@CheckoutAttributeDescription", CheckoutAttributeDescription);
                paramDic.Add("@CheckoutAttributesXml", CheckoutAttributesXml);
                paramDic.Add("@CustomerLanguageId", CustomerLanguageId);
                paramDic.Add("@AffiliateId", AffiliateId);
                paramDic.Add("@CustomerIp", CustomerIp);
                paramDic.Add("@CardType", CardType);
                paramDic.Add("@CardName", CardName);
                paramDic.Add("@MaskedCreditCardNumber", MaskedCreditCardNumber);
                paramDic.Add("@CardExpirationMonth", CardExpirationMonth);
                paramDic.Add("@CardExpirationYear", CardExpirationYear);
                paramDic.Add("@ShippingMethod", ShippingMethod);
                paramDic.Add("@ShippingRateComputationMethodSystemName", ShippingRateComputationMethodSystemName);
                paramDic.Add("@CustomValuesXml", CustomValuesXml);
                paramDic.Add("@Errors", errors);
                paramDic.Add("@OrderGuid", paymentRequest.OrderGuid);

                insert = "EXEC usp_InsertFailedOrder @GBSOrderID,@StoreID,@CustomerId,@BillingAddressId,@ShippingAddressId,@PickupAddressId,@PickUpInStore,@PaymentMethodSystemName,@CustomerCurrencyCode,@CurrencyRate,@CustomerTaxDisplayTypeId,@VatNumber,@OrderSubtotalInclTax,";
                insert += "@OrderSubtotalExclTax,@OrderSubTotalDiscountInclTax,@OrderSubTotalDiscountExclTax,@OrderShippingInclTax,@OrderShippingExclTax,@PaymentMethodAdditionalFeeInclTax,@PaymentMethodAdditionalFeeExclTax,@TaxRates,@OrderTax,@OrderDiscount,@OrderTotal,@RewardPointsWereAdded,@CheckoutAttributeDescription,";
                insert += "@CheckoutAttributesXml,@CustomerLanguageId,@AffiliateId,@CustomerIp,@CardType,@CardName,@MaskedCreditCardNumber,@CardExpirationMonth,@CardExpirationYear,@ShippingMethod,@ShippingRateComputationMethodSystemName,@CustomValuesXml,@Errors,@OrderGuid";

                manager.SetParameterizedQueryNoData(insert, paramDic);

            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error in Order Service saving FailedOrder - insert = "+ insert, ex, customer);
            }


            var cart = orderContainer.Cart;

            try
            {

                DBManager manager = new DBManager();
                //var cart = _workContext.CurrentCustomer.ShoppingCartItems;
                //var cartQuery = from item in cart where item.ShoppingCartType == ShoppingCartType.ShoppingCart && item.StoreId == paymentRequest.StoreId select item;
                foreach (ShoppingCartItem item in cart) {
                    decimal taxRate;
                    List<DiscountForCaching> scDiscounts;
                    decimal discountAmount;
                    var scUnitPrice = _priceCalculationService.GetUnitPrice(item);
                    int? maximumDiscountQty;
                    var scSubTotal = _priceCalculationService.GetSubTotal(item, true, out discountAmount, out scDiscounts, out maximumDiscountQty);
                    int StoreID = paymentRequest.StoreId;
                    int CustomerId = paymentRequest.CustomerId;
                    int ProductID = item.ProductId;
                    int Quantity = item.Quantity;
                    string AttributesXml = item.AttributesXml != null ? item.AttributesXml : String.Empty;
                    var AttributeDescription = _productAttributeFormatter.FormatAttributes(item.Product, item.AttributesXml, orderContainer.Customer);
                    Customer customer = _customerService.GetCustomerById(CustomerId);

                    decimal discountAmountInclTax = _taxService.GetProductPrice(item.Product, discountAmount, true, customer, out taxRate);
                    decimal discountAmountExclTax = _taxService.GetProductPrice(item.Product, discountAmount, false, customer, out taxRate);
                    decimal OriginalProductCost = _priceCalculationService.GetProductCost(item.Product, item.AttributesXml);
                    decimal PriceInclTax = _taxService.GetProductPrice(item.Product, scSubTotal, true, customer, out taxRate);
                    decimal PriceExclTax = _taxService.GetProductPrice(item.Product, scSubTotal, true, customer, out taxRate);
                    DateTime? RentalEndDateUtc = item.RentalEndDateUtc;
                    DateTime? RentalStartDateUtc = item.RentalStartDateUtc;
                    decimal UnitPriceExclTax = _taxService.GetProductPrice(item.Product, scUnitPrice, false, customer, out taxRate);
                    decimal UnitPriceInclTax = _taxService.GetProductPrice(item.Product, scUnitPrice, true, customer, out taxRate);
                    int ccID = 0;
                    bool isCCProduct = ccService.IsProductForCc(item.ProductId);
                    if (isCCProduct)
                    {
                        ccID = getCCIdFromItem(item.AttributesXml);
                    }



                    Dictionary<string, Object> paramDic = new Dictionary<string, Object>();
                    paramDic.Add("@GBSOrderID", GBSOrderID);
                    paramDic.Add("@StoreID", StoreID);
                    paramDic.Add("@CustomerId", CustomerId);
                    paramDic.Add("@ProductID", ProductID);
                    paramDic.Add("@Quantity", Quantity);
                    paramDic.Add("@AttributesXml", AttributesXml);
                    paramDic.Add("@AttributeDescription", AttributeDescription);

                    paramDic.Add("@discountAmountInclTax", discountAmountInclTax);
                    paramDic.Add("@discountAmountExclTax", discountAmountExclTax);
                    paramDic.Add("@OriginalProductCost", OriginalProductCost);
                    paramDic.Add("@PriceInclTax", PriceInclTax);
                    paramDic.Add("@PriceExclTax", PriceExclTax);
                    //paramDic.Add("@RentalEndDateUtc", RentalEndDateUtc);
                    //paramDic.Add("@RentalStartDateUtc", RentalStartDateUtc);
                    paramDic.Add("@UnitPriceExclTax", UnitPriceExclTax);
                    paramDic.Add("@UnitPriceInclTax", UnitPriceInclTax);
                    paramDic.Add("@ccID", ccID);

                    insert = "EXEC usp_InsertFailedOrderItems @GBSOrderID,@StoreID,@CustomerId,@ProductID,@Quantity,@AttributesXml,@AttributeDescription,@discountAmountInclTax,@discountAmountExclTax,@OriginalProductCost,@PriceInclTax,@PriceExclTax,@UnitPriceExclTax,@UnitPriceInclTax,@ccID";

                    manager.SetParameterizedQueryNoData(insert, paramDic);


                }


            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error in Order Service saving FailedOrderItem - insert = " + insert, ex, customer);
            }

            //upload and process files.

            List<ExtendedOrderItem> extendedOrderItems = new List<ExtendedOrderItem>();
            List<ProductFileModel> ccFiles = new List<ProductFileModel>();

            foreach (ShoppingCartItem item in cart)
            {
                ExtendedOrderItem extendedOrderItem = new ExtendedOrderItem();
                extendedOrderItem.OrderItemID = item.Id;
                bool isCCProduct = ccService.IsProductForCc(item.ProductId);
                if (isCCProduct)
                {
                    ProcessCanvasProducts(convertSiToOi(item), extendedOrderItem, ccFiles, true);
                }
                extendedOrderItems.Add(extendedOrderItem);
            }


            //call file service to save files
            SaveProductionFiles(ccFiles, GBSOrderID, true);


        }
        public OrderItem convertSiToOi(ShoppingCartItem si)
        {
            OrderItem oi = new OrderItem();
            oi.AttributesXml = si.AttributesXml;
            //oi.Id = si.Id;
            oi.OrderItemGuid = Guid.NewGuid();
            oi.Product = si.Product;
            oi.ProductId = si.ProductId;
            oi.Quantity = si.Quantity;
            oi.RentalEndDateUtc = si.RentalEndDateUtc;
            oi.RentalStartDateUtc = si.RentalStartDateUtc;
            return oi;
        }
    }
    public class ExtendedOrderItem
    {
        public int ID { get; set; }
        public int OrderItemID { get; set; }
        public int ccID { get; set; }
    }



}
