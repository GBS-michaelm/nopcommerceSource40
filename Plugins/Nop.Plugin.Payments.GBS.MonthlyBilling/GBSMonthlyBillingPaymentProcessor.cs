using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling
{
    /// <summary>
    /// MonthlyBilling payment processor
    /// </summary>
    public class GBSMonthlyBillingPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly GBSMonthlyBillingPaymentSettings _monthlyBillingPaymentSettings;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public GBSMonthlyBillingPaymentProcessor(ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            GBSMonthlyBillingPaymentSettings purchaseOrderPaymentSettings,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._monthlyBillingPaymentSettings = purchaseOrderPaymentSettings;
            this._webHelper = webHelper;
            this._httpContextAccessor = httpContextAccessor;
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
            result.NewPaymentStatus = PaymentStatus.Pending;
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
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country

            if (_monthlyBillingPaymentSettings.ShippableProductRequired && !cart.RequiresShipping())
                return true;

            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
             _monthlyBillingPaymentSettings.AdditionalFee, _monthlyBillingPaymentSettings.AdditionalFeePercentage);          
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
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
            return _webHelper.GetStoreLocation() + "Admin/GBSPaymentMonthlyBilling/Configure";
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// List of validating errors
        /// </returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// Payment info holder
        /// </returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            //paymentInfo.CustomValues.Add("PO Number", form["PurchaseOrderNumber"]);
            //paymentInfo.CustomValues.Add("PO Name", form["PurchaseOrderName"]);
            //paymentInfo.CustomValues.Add("PO Phone", form["PurchaseOrderPhoneNumber"]);
            _httpContextAccessor.HttpContext.Session.SetString("monthlyBillingName", form["MonthlyBillingName"]);
            _httpContextAccessor.HttpContext.Session.SetString("monthlyBillingPhoneNumber", form["MonthlyBillingPhoneNumber"]);
            _httpContextAccessor.HttpContext.Session.SetString("monthlyBillingReference", form["MonthlyBillingReference"]);
            return paymentInfo;
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentGBSMonthlyBilling";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new GBSMonthlyBillingPaymentSettings());

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFee.Hint", "The additional fee.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.PaymentMethodDescription", "Pay by monthly billing");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingReference", "Monthly Billing Reference");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingName", "Monthly Billing Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingPhoneNumber", "Monthly Billing Phone");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.ShippableProductRequired", "Shippable product required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.MonthlyBilling.ShippableProductRequired.Hint", "An option indicating whether shippable products are required in order to display this payment method during checkout.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GBSMonthlyBillingPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.PaymentMethodDescription");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingReference");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingName");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.MonthlyBillingPhoneNumber");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.ShippableProductRequired");
            this.DeletePluginLocaleResource("Plugins.Payment.MonthlyBilling.ShippableProductRequired.Hint");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Standard; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payment.MonthlyBilling.PaymentMethodDescription"); }
        }

        #endregion

    }
}
