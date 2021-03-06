﻿using Microsoft.AspNetCore.Http;
//using AuthorizeNet.Api.Contracts.V1;
//using AuthorizeNet.Api.Controllers;
//using AuthorizeNet.Api.Controllers.Bases;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.GBS.Controllers;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Plugin.Payments.GBS.Validators;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework.Mvc.ModelBinding;
using Payment.GBSPaymentGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using WebServices.Models.Payment;

//using AuthorizeNetSDK = AuthorizeNet;

namespace Nop.Plugin.Payments.GBS
{
    /// <summary>
    /// GBS payment processor
    /// </summary>
    public class GBSPaymentGateway : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IEncryptionService _encryptionService;
        private readonly CurrencySettings _currencySettings;
        private readonly GBSPaymentSettings _gbsPaymentSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public GBSPaymentGateway(ISettingService settingService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IWebHelper webHelper,
            IOrderTotalCalculationService orderTotalCalculationService,
            IEncryptionService encryptionService,
            CurrencySettings currencySettings,
            GBSPaymentSettings gbsPaymentSettings,
            ILocalizationService localizationService,
            ILogger logger)
        {
            this._gbsPaymentSettings = gbsPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._encryptionService = encryptionService;
            this._localizationService = localizationService;
            this._logger = logger;
        }

        #endregion

        #region Utilities

        private void PrepareGBS()
        {
            //ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = _authorizeNetPaymentSettings.UseSandbox
            //    ? AuthorizeNetSDK.Environment.SANDBOX
            //    : AuthorizeNetSDK.Environment.PRODUCTION;

            //// define the merchant information (authentication / transaction id)
            //ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            //{
            //    name = _authorizeNetPaymentSettings.LoginId,
            //    ItemElementName = ItemChoiceType.transactionKey,
            //    Item = _authorizeNetPaymentSettings.TransactionKey,
            //};
        }
        public class message
        {
            public message() { }
            private string _description;
            private string _text;
            private string _code;
            public string description
            {
                get
                {
                    return _description;
                }
                set
                {
                    _description = value;
                }
            }
            public string text
            {
                get
                {
                    return _text;
                }
                set
                {
                    _text = value;
                }
            }
            public string code
            {
                get
                {
                    return _code;
                }
                set
                {
                    _code = value;
                }
            }
        }
        
        private static bool GetErrors(GBSTransactionResponse response, IList<string> errors)
        {
            var rezult = false;
            if (response != null && response.resultCode == GBSTransactionResponse.ResultCodeType.Success)
            {
                switch (response.responseCode)
                {
                    case GBSTransactionResponse.ResponseCodeType.Approved:
                        break;
                    case GBSTransactionResponse.ResponseCodeType.Declined:
                        {
                            errors.Add(string.Format("Declined ({0}: {1})", response.responseCode,
                                response.messages != null ? response.messages[0] : ""));
                            rezult = true;
                        }
                        break;
                    case GBSTransactionResponse.ResponseCodeType.Error:
                        {
                            foreach (var transactionResponseError in response.errors)
                            {
                                errors.Add(string.Format("Error #{0}: {1}", response.responseCode,
                                    response.errors != null ? response.errors[0] : ""));
                            }
                            rezult = true;
                        }
                        break;
                    default:
                        {
                            errors.Add("GBS unknown error");
                            rezult = true;
                        }
                        break;
                }

                return rezult;
            }

            if (response != null)
            {
                foreach (var responseMessage in response.messages)
                {
                    errors.Add(string.Format("Error #{0}: {1}", response.responseCode, responseMessage));
                }

                if (response.errors != null)
                {
                    foreach (var transactionResponseError in response.errors)
                    {
                        errors.Add(string.Format("Error #{0}: {1}", response.responseCode, transactionResponseError));
                    }
                }
            }
            else
            {
                errors.Add("Authorize.NET unknown error");
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
           
            try
            {

           

            PrepareGBS();

            //var creditCard = new creditCardType
            //{
            //    cardNumber = processPaymentRequest.CreditCardNumber,
            //    expirationDate =
            //        processPaymentRequest.CreditCardExpireMonth.ToString("D2") + processPaymentRequest.CreditCardExpireYear,
            //    cardCode = processPaymentRequest.CreditCardCvv2
            //};

            ////standard api call to retrieve response
            //var paymentType = new paymentType { Item = creditCard };

            //transactionTypeEnum transactionType;

            //switch (_authorizeNetPaymentSettings.TransactMode)
            //{
            //    case TransactMode.Authorize:
            //        transactionType = transactionTypeEnum.authOnlyTransaction;
            //        break;
            //    case TransactMode.AuthorizeAndCapture:
            //        transactionType = transactionTypeEnum.authCaptureTransaction;
            //        break;
            //    default:
            //        throw new NopException("Not supported transaction mode");
            //}

            //var billTo = new customerAddressType
            //{
            //    firstName = customer.BillingAddress.FirstName,
            //    lastName = customer.BillingAddress.LastName,
            //    email = customer.BillingAddress.Email,
            //    address = customer.BillingAddress.Address1,
            //    city = customer.BillingAddress.City,
            //    zip = customer.BillingAddress.ZipPostalCode
            //};

            //if (!string.IsNullOrEmpty(customer.BillingAddress.Company))
            //    billTo.company = customer.BillingAddress.Company;

            //if (customer.BillingAddress.StateProvince != null)
            //    billTo.state = customer.BillingAddress.StateProvince.Abbreviation;

            //if (customer.BillingAddress.Country != null)
            //    billTo.country = customer.BillingAddress.Country.TwoLetterIsoCode;

            //var transactionRequest = new transactionRequestType
            //{
            //    transactionType = transactionType.ToString(),
            //    amount = Math.Round(processPaymentRequest.OrderTotal, 2),
            //    payment = paymentType,
            //    currencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
            //    billTo = billTo,
            //    customerIP = _webHelper.GetCurrentIpAddress(),
            //    order = new orderType
            //    {
            //        //x_invoice_num is 20 chars maximum. hece we also pass x_description
            //        invoiceNumber = processPaymentRequest.OrderGuid.ToString().Substring(0, 20),
            //        description = string.Format("Full order #{0}", processPaymentRequest.OrderGuid)
            //    }
            //};

            //var request = new createTransactionRequest { transactionRequest = transactionRequest };

            //// instantiate the contoller that will call the service
            //var controller = new createTransactionController(request);
            //controller.Execute();

            //// get the response from the service (errors contained if any)




            //NOP WEB SERVICE CALL START HERE --------------------------------------------------------------------------------------------------------------------
            //create nop payment object from user data
            PaymentTransactionModel payment = new PaymentTransactionModel();
            payment.firstName = customer.BillingAddress.FirstName;
            payment.lastName = customer.BillingAddress.LastName;
            payment.streetAddress = customer.BillingAddress.Address1;
            payment.billingCity = customer.BillingAddress.City;
            payment.postalCode = customer.BillingAddress.ZipPostalCode;
            payment.countryCode = customer.BillingAddress.Country.Name;
            if (payment.countryCode == "United States")
            {
                payment.countryCode = "US";
            }
            else
            {
                payment.countryCode = "US";
            }
            payment.cardExpireMonth = processPaymentRequest.CreditCardExpireMonth.ToString().Length == 1 ? "0"+processPaymentRequest.CreditCardExpireMonth.ToString() : processPaymentRequest.CreditCardExpireMonth.ToString(); //prepend 0 for single digit months
            payment.cardExpireYear = processPaymentRequest.CreditCardExpireYear.ToString();
            payment.cardNum = processPaymentRequest.CreditCardNumber;
            payment.orderAmount = processPaymentRequest.OrderTotal.ToString();
            payment.pcDestZip = (customer.ShippingAddress != null && customer.ShippingAddress.ZipPostalCode != null) ? customer.ShippingAddress.ZipPostalCode : String.Empty;
            Object value = null;
            NopResourceDisplayNameAttribute orderNumberKeyGBS = new NopResourceDisplayNameAttribute(("Account.CustomerOrders.OrderNumber"));
            if (processPaymentRequest.CustomValues.TryGetValue(orderNumberKeyGBS.DisplayName, out value))
            {
                payment.orderID = processPaymentRequest.CustomValues[orderNumberKeyGBS.DisplayName].ToString();
                payment.pcOrderID = processPaymentRequest.CustomValues[orderNumberKeyGBS.DisplayName].ToString();
            }
            else
            {
                payment.orderID = "NA";
                payment.pcOrderID = "NA";
            }
            payment.state = customer.BillingAddress.StateProvince.Abbreviation;
            payment.tax = _orderTotalCalculationService.GetTaxTotal((IList<ShoppingCartItem>)customer.ShoppingCartItems, false).ToString();
            payment.sandBox = _gbsPaymentSettings.UseSandbox;


            if (Convert.ToBoolean(processPaymentRequest.CustomValues["SavedProfile"]) == true)
            {
                payment.createProfile = false;
                payment.useProfile = true;
                payment.profileID = processPaymentRequest.CustomValues["ProfileID"].ToString();
            }
            else
            {
                payment.createProfile = true;
                payment.useProfile = false;
                payment.profileID = "";
            }
                

            //will need to be able to switch between sand and production version
            string address = _gbsPaymentSettings.GBSPaymentWebServiceAddress;



                GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();
            
            //calls to GBSPaymentGateway function       
            var response = gateway.AuthorizeAndCapture(payment, address, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password);


            //use returned GBSTransactionResponse to tell nop what occured in submit
            switch (response.responseCode)
            {
                case GBSTransactionResponse.ResponseCodeType.Approved:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case GBSTransactionResponse.ResponseCodeType.Declined:
                case GBSTransactionResponse.ResponseCodeType.Error:
                    result.NewPaymentStatus = PaymentStatus.Voided;
                    break;
                default:
                    result.NewPaymentStatus = PaymentStatus.Voided;
                    break;
            }
            //validate
            if (GetErrors(response, result.Errors))
                return result;

            if (_gbsPaymentSettings.TransactMode == TransactMode.Authorize)
                result.AuthorizationTransactionCode = string.Format("{0},{1}", response.transactId, response.authCode);
                //if (_authorizeNetPaymentSettings.TransactMode == TransactMode.AuthorizeAndCapture)
                //    result.CaptureTransactionId = string.Format("{0},{1}", response.transactionResponse.transId, response.transactionResponse.authCode);

                result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.responseCode, response.authCode);
                //result.AvsResult = response.transactionResponse.avsResultCode;
                //result.NewPaymentStatus = _authorizeNetPaymentSettings.TransactMode == TransactMode.Authorize ? PaymentStatus.Authorized : PaymentStatus.Paid;

                //var congrats = "Congrats the payment was successful, now insert profile id into DB";

                bool storeProfile = Convert.ToBoolean(processPaymentRequest.CustomValues["StoreProfile"]);

                if (payment.createProfile == true && storeProfile == true)
                {
                    string nickName = String.IsNullOrEmpty(processPaymentRequest.CustomValues["NickName"].ToString()) ? "" : processPaymentRequest.CustomValues["NickName"].ToString();
                    nickName = nickName.Replace("'", "''");

                    string last4Digits = response.accountNum.ToString();
                    last4Digits = last4Digits.Substring(last4Digits.Length - 4);

                    DBManager manager = new DBManager();
                    Dictionary<string, string> paramDic = new Dictionary<string, string>();
                    paramDic.Add("@CustomerID", customer.Id.ToString());
                    paramDic.Add("@ProfileID", response.customerRefNum.ToString());
                    paramDic.Add("@NickName", nickName);
                    paramDic.Add("@Last4Digits", last4Digits);
                    paramDic.Add("@CardType", response.cardBrand.ToString());
                    paramDic.Add("@ExpMonth", payment.cardExpireMonth);
                    paramDic.Add("@ExpYear", payment.cardExpireYear);
                    
                    string insert = "INSERT INTO Profiles (CustomerID, ProfileID, NickName, Last4Digits, CardType, ExpMonth, ExpYear) ";
                    insert += "VALUES ('" + customer.Id + "', '" + response.customerRefNum + "', '" + nickName + "', '" + last4Digits + "', '" + response.cardBrand + "', '" + payment.cardExpireMonth + "', '" + payment.cardExpireYear + "')";
                    try
                    {
                        manager.SetParameterizedQueryNoData(insert, paramDic);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            _logger.Error("Payment Plugin Error inserting profile on auth : " + ex.Message + ": query = " + insert, ex, null);
                        }
                        catch (Exception ex1) {
                            _logger.Error("Payment Plugin Error inserting profile on auth with additional failure to log the sql statement : " + ex.Message, ex, null);
                        }
                    }

                }

            }
            catch(Exception ex)
            {
                _logger.Error("Payment Plugin Error : " + ex.Message, ex, null);
                throw new Exception("Payment Plugin Exception: "+ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            //var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
            //    _gbsPaymentSettings.AdditionalFee, _gbsPaymentSettings.AdditionalFeePercentage);
            //return result;
            return 0;
            // throw new Exception("this method not implemented");
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            //var result = new CapturePaymentResult();

            //PrepareAuthorizeNet();

            //var codes = capturePaymentRequest.Order.AuthorizationTransactionCode.Split(',');
            //var transactionRequest = new transactionRequestType
            //{
            //    transactionType = transactionTypeEnum.priorAuthCaptureTransaction.ToString(),
            //    amount = Math.Round(capturePaymentRequest.Order.OrderTotal, 2),
            //    refTransId = codes[0],
            //    currencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
            //};

            //var request = new createTransactionRequest { transactionRequest = transactionRequest };

            //// instantiate the contoller that will call the service
            //var controller = new createTransactionController(request);
            //controller.Execute();

            //// get the response from the service (errors contained if any)
            //var response = controller.GetApiResponse();

            ////validate
            //if (GetErrors(response, result.Errors))
            //    return result;

            //result.CaptureTransactionId = string.Format("{0},{1}", response.transactionResponse.transId, response.transactionResponse.authCode);
            //result.CaptureTransactionResult = string.Format("Approved ({0}: {1})", response.transactionResponse.responseCode, response.transactionResponse.messages[0].description);
            //result.NewPaymentStatus = PaymentStatus.Paid;

            //return result;
            throw new Exception("this method not implemented");

        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            //var result = new RefundPaymentResult();

            //PrepareAuthorizeNet();

            //var maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(refundPaymentRequest.Order.MaskedCreditCardNumber);

            //if (String.IsNullOrEmpty(maskedCreditCardNumberDecrypted) || maskedCreditCardNumberDecrypted.Length < 4)
            //{
            //    result.AddError("Last four digits of Credit Card Not Available");
            //    return result;
            //}

            //var lastFourDigitsCardNumber = maskedCreditCardNumberDecrypted.Substring(maskedCreditCardNumberDecrypted.Length - 4);
            //var creditCard = new creditCardType
            //{
            //    cardNumber = lastFourDigitsCardNumber,
            //    expirationDate = "XXXX"
            //};

            //var codes = (string.IsNullOrEmpty(refundPaymentRequest.Order.CaptureTransactionId) ? refundPaymentRequest.Order.AuthorizationTransactionCode : refundPaymentRequest.Order.CaptureTransactionId).Split(',');
            //var transactionRequest = new transactionRequestType
            //{
            //    transactionType = transactionTypeEnum.refundTransaction.ToString(),
            //    amount = Math.Round(refundPaymentRequest.AmountToRefund, 2),
            //    refTransId = codes[0],
            //    currencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,

            //    order = new orderType
            //    {
            //        //x_invoice_num is 20 chars maximum. hece we also pass x_description
            //        invoiceNumber = refundPaymentRequest.Order.OrderGuid.ToString().Substring(0, 20),
            //        description = string.Format("Full order #{0}", refundPaymentRequest.Order.OrderGuid)
            //    },

            //    payment = new paymentType { Item = creditCard }
            //};

            //var request = new createTransactionRequest { transactionRequest = transactionRequest };

            //// instantiate the contoller that will call the service
            //var controller = new createTransactionController(request);
            //controller.Execute();

            //// get the response from the service (errors contained if any)
            //var response = controller.GetApiResponse();

            ////validate
            //if (GetErrors(response, result.Errors))
            //    return result;

            //var isOrderFullyRefunded = refundPaymentRequest.AmountToRefund + refundPaymentRequest.Order.RefundedAmount == refundPaymentRequest.Order.OrderTotal;
            //result.NewPaymentStatus = isOrderFullyRefunded ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;

            //return result;
            throw new Exception("this method not implemented");

        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            //var result = new VoidPaymentResult();

            //PrepareAuthorizeNet();

            //var maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(voidPaymentRequest.Order.MaskedCreditCardNumber);

            //if (String.IsNullOrEmpty(maskedCreditCardNumberDecrypted) || maskedCreditCardNumberDecrypted.Length < 4)
            //{
            //    result.AddError("Last four digits of Credit Card Not Available");
            //    return result;
            //}

            //var lastFourDigitsCardNumber = maskedCreditCardNumberDecrypted.Substring(maskedCreditCardNumberDecrypted.Length - 4);
            //var expirationDate = voidPaymentRequest.Order.CardExpirationMonth + voidPaymentRequest.Order.CardExpirationYear;

            //if (!expirationDate.Any() && _authorizeNetPaymentSettings.UseSandbox)
            //    expirationDate = DateTime.Now.ToString("MMyyyy");

            //var creditCard = new creditCardType
            //{
            //    cardNumber = lastFourDigitsCardNumber,
            //    expirationDate = expirationDate
            //};

            //var codes = (string.IsNullOrEmpty(voidPaymentRequest.Order.CaptureTransactionId) ? voidPaymentRequest.Order.AuthorizationTransactionCode : voidPaymentRequest.Order.CaptureTransactionId).Split(',');
            //var transactionRequest = new transactionRequestType
            //{
            //    transactionType = transactionTypeEnum.voidTransaction.ToString(),
            //    refTransId = codes[0],
            //    payment = new paymentType { Item = creditCard }
            //};

            //var request = new createTransactionRequest { transactionRequest = transactionRequest };

            //// instantiate the contoller that will call the service
            //var controller = new createTransactionController(request);
            //controller.Execute();

            //// get the response from the service (errors contained if any)
            //var response = controller.GetApiResponse();

            ////validate
            //if (GetErrors(response, result.Errors))
            //    return result;

            //result.NewPaymentStatus = PaymentStatus.Voided;

            //return result;
            throw new Exception("this method not implemented");
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            //var result = new ProcessPaymentResult();

            //if (processPaymentRequest.IsRecurringPayment) return result;

            //var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);

            //PrepareAuthorizeNet();

            //var creditCard = new creditCardType
            //{
            //    cardNumber = processPaymentRequest.CreditCardNumber,
            //    expirationDate =
            //        processPaymentRequest.CreditCardExpireMonth.ToString("D2") + processPaymentRequest.CreditCardExpireYear,
            //    cardCode = processPaymentRequest.CreditCardCvv2
            //};

            ////standard api call to retrieve response
            //var paymentType = new paymentType { Item = creditCard };

            //var billTo = new nameAndAddressType
            //{
            //    firstName = customer.BillingAddress.FirstName,
            //    lastName = customer.BillingAddress.LastName,
            //    //email = customer.BillingAddress.Email,
            //    address = customer.BillingAddress.Address1,
            //    //address = customer.BillingAddress.Address1 + " " + customer.BillingAddress.Address2;
            //    city = customer.BillingAddress.City,
            //    zip = customer.BillingAddress.ZipPostalCode
            //};

            //if (!string.IsNullOrEmpty(customer.BillingAddress.Company))
            //    billTo.company = customer.BillingAddress.Company;

            //if (customer.BillingAddress.StateProvince != null)
            //    billTo.state = customer.BillingAddress.StateProvince.Abbreviation;

            //if (customer.BillingAddress.Country != null)
            //    billTo.country = customer.BillingAddress.Country.TwoLetterIsoCode;

            //var dtNow = DateTime.UtcNow;

            //// Interval can't be updated once a subscription is created.
            //var paymentScheduleInterval = new paymentScheduleTypeInterval();
            //switch (processPaymentRequest.RecurringCyclePeriod)
            //{
            //    case RecurringProductCyclePeriod.Days:
            //        paymentScheduleInterval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength);
            //        paymentScheduleInterval.unit = ARBSubscriptionUnitEnum.days;
            //        break;
            //    case RecurringProductCyclePeriod.Weeks:
            //        paymentScheduleInterval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength * 7);
            //        paymentScheduleInterval.unit = ARBSubscriptionUnitEnum.days;
            //        break;
            //    case RecurringProductCyclePeriod.Months:
            //        paymentScheduleInterval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength);
            //        paymentScheduleInterval.unit = ARBSubscriptionUnitEnum.months;
            //        break;
            //    case RecurringProductCyclePeriod.Years:
            //        paymentScheduleInterval.length = Convert.ToInt16(processPaymentRequest.RecurringCycleLength * 12);
            //        paymentScheduleInterval.unit = ARBSubscriptionUnitEnum.months;
            //        break;
            //    default:
            //        throw new NopException("Not supported cycle period");
            //}

            //var paymentSchedule = new paymentScheduleType
            //{
            //    startDate = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day),
            //    totalOccurrences = Convert.ToInt16(processPaymentRequest.RecurringTotalCycles),
            //    interval = paymentScheduleInterval
            //};

            //var subscriptionType = new ARBSubscriptionType
            //{
            //    name = processPaymentRequest.OrderGuid.ToString(),
            //    amount = Math.Round(processPaymentRequest.OrderTotal, 2),
            //    payment = paymentType,
            //    billTo = billTo,
            //    paymentSchedule = paymentSchedule,
            //    customer = new customerType
            //    {
            //        email = customer.BillingAddress.Email
            //        //phone number should be in one of following formats: 111- 111-1111 or (111) 111-1111.
            //        //phoneNumber = customer.BillingAddress.PhoneNumber
            //    },

            //    order = new orderType
            //    {
            //        //x_invoice_num is 20 chars maximum. hece we also pass x_description
            //        invoiceNumber = processPaymentRequest.OrderGuid.ToString().Substring(0, 20),
            //        description = String.Format("Recurring payment #{0}", processPaymentRequest.OrderGuid)
            //    }
            //};

            //if (customer.ShippingAddress != null)
            //{
            //    var shipTo = new nameAndAddressType
            //    {
            //        firstName = customer.ShippingAddress.FirstName,
            //        lastName = customer.ShippingAddress.LastName,
            //        address = customer.ShippingAddress.Address1,
            //        city = customer.ShippingAddress.City,
            //        zip = customer.ShippingAddress.ZipPostalCode
            //    };

            //    if (customer.ShippingAddress.StateProvince != null)
            //    {
            //        shipTo.state = customer.ShippingAddress.StateProvince.Abbreviation;
            //    }

            //    subscriptionType.shipTo = shipTo;
            //}

            //var request = new ARBCreateSubscriptionRequest { subscription = subscriptionType };

            //// instantiate the contoller that will call the service
            //var controller = new ARBCreateSubscriptionController(request);
            //controller.Execute();

            //// get the response from the service (errors contained if any)
            //var response = controller.GetApiResponse();

            ////validate
            //if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            //{
            //    result.SubscriptionTransactionId = response.subscriptionId;
            //    result.AuthorizationTransactionCode = response.refId;
            //    result.AuthorizationTransactionResult = string.Format("Approved ({0}: {1})", response.refId, response.subscriptionId);
            //}
            //else if (response != null)
            //{
            //    foreach (var responseMessage in response.messages.message)
            //    {
            //        result.AddError(string.Format("Error processing recurring payment #{0}: {1}", responseMessage.code, responseMessage.text));
            //    }
            //}
            //else
            //{
            //    result.AddError("Authorize.NET unknown error");
            //}

            //return result;
            throw new Exception("this method not implemented");
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            //var result = new CancelRecurringPaymentResult();
            //PrepareAuthorizeNet();

            //var request = new ARBCancelSubscriptionRequest { subscriptionId = cancelPaymentRequest.Order.SubscriptionTransactionId };
            //var controller = new ARBCancelSubscriptionController(request);
            //controller.Execute();

            //var response = controller.GetApiResponse();

            ////validate
            //if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            //    return result;

            //if (response != null)
            //{
            //    foreach (var responseMessage in response.messages.message)
            //    {
            //        result.AddError(string.Format("Error processing recurring payment #{0}: {1}", responseMessage.code, responseMessage.text));
            //    }
            //}
            //else
            //{
            //    result.AddError("Authorize.NET unknown error");
            //}

            //return result;
            throw new Exception("this method not implemented");
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

         /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentGBS/Configure";
        }      

        public Type GetControllerType()
        {
            return typeof(PaymentGBSController);
        }

        public override void Install()
        {
            //settings
            var settings = new GBSPaymentSettings
            {
                UseSandbox = true,
                UseSavedCards = true,
                TransactMode = TransactMode.Authorize,
                //TransactionKey = "123",
                LoginId = ""
            };
            _settingService.SaveSetting(settings);

            //locales
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Notes", "If you're using this gateway, ensure that your primary store currency is supported by Authorize.NET.");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.UseSandbox", "Use Sandbox");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactModeValues", "Transaction mode");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactModeValues.Hint", "Choose transaction mode.");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactionKey", "Transaction key");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactionKey.Hint", "Specify transaction key.");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.LoginId", "Login ID");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.LoginId.Hint", "Specify login identifier.");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFee", "Additional fee");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GBSPaymentSettings>();

            //locales
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Notes");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.UseSandbox");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.UseSandbox.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactModeValues");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactModeValues.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactionKey");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.TransactionKey.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.LoginId");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.LoginId.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFee");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFee.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage");
            //this.DeletePluginLocaleResource("Plugins.Payments.AuthorizeNet.Fields.AdditionalFeePercentage.Hint");

            base.Uninstall();
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();

            try
            {
                

                //validate
                var validator = new PaymentInfoValidator(_localizationService);


                var model = new PaymentInfoModel
                {
                    CardholderName = form["CardholderName"],
                    CardNumber = form["CardNumber"],
                    CardCode = form["CardCode"],
                    ExpireMonth = form["ExpireMonth"],
                    ExpireYear = form["ExpireYear"]
                };

                var validationResult = validator.Validate(model);
                var validationCheck = validationResult.IsValid;

                if (form.Keys.Contains("ProfileID") && !String.IsNullOrEmpty(form["ProfileID"].ToString()))
                {
                    validationCheck = true;
                }

                if (!validationCheck)
                    warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            catch (Exception ex)
            {
                var innerMsg = "";
               if (ex.InnerException != null)
                {
                    innerMsg = ex.InnerException.Message;
                }

                _logger.Error("error in ValidatePaymentForm() --- " +ex.Message + " inner exception " +innerMsg, ex, null);
                throw ex;
            }

            return warnings;
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();

            GBSPaymentServiceClient gateway = new GBSPaymentServiceClient();

            if (form.Keys.Contains("ProfileID") && !String.IsNullOrEmpty(form["ProfileID"].ToString()))
            {
                WebServices.Models.Payment.PaymentProfileModel paymentProfile = gateway.ReadProfile(int.Parse(form["ProfileID"]), _gbsPaymentSettings.GBSPaymentWebServiceAddress, _gbsPaymentSettings.LoginId, _gbsPaymentSettings.Password, _gbsPaymentSettings.UseSandbox);

                paymentInfo = new ProcessPaymentRequest();

                paymentInfo.CreditCardName = paymentProfile.customerName;
                paymentInfo.CreditCardNumber = paymentProfile.cardNumber;
                paymentInfo.CreditCardExpireMonth = int.Parse(paymentProfile.cardExpireMonth);
                paymentInfo.CreditCardExpireYear = int.Parse(paymentProfile.cardExpireYear);
                paymentInfo.CustomValues["ProfileID"] = form["ProfileID"].ToString();
                paymentInfo.CustomValues["SavedProfile"] = true;
                paymentInfo.CustomValues["StoreProfile"] = form["StoreProfile"];
            }
            else
            {
                paymentInfo = new ProcessPaymentRequest();

                //paymentInfo.CreditCardType is not used by Authorize.NET
                paymentInfo.CreditCardName = form["CardholderName"];
                paymentInfo.CreditCardNumber = form["CardNumber"];
                paymentInfo.CreditCardExpireMonth = int.Parse(form["ExpireMonth"]);
                paymentInfo.CreditCardExpireYear = int.Parse(form["ExpireYear"]);
                paymentInfo.CreditCardCvv2 = form["CardCode"];
                paymentInfo.CustomValues["SavedProfile"] = false;
                paymentInfo.CustomValues["NickName"] = form["NickName"];
                paymentInfo.CustomValues["StoreProfile"] = form["StoreProfile"];
            }

            return paymentInfo;
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentGBS";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.Manual;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        public string PaymentMethodDescription
        {
            get
            {
                return _localizationService.GetResource("Plugins.Payment.GBSPaymentGateway.PaymentMethodDescription");
            }
            
        }
        
        #endregion
    }
}
