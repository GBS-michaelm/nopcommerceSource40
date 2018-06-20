using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.MonthlyBilling.MonthlyBillingReference")]
        public string MonthlyBillingReference { get; set; }

        [NopResourceDisplayName("Plugins.Payment.MonthlyBilling.MonthlyBillingName")]
        public string MonthlyBillingName { get; set; }

        [NopResourceDisplayName("Plugins.Payment.MonthlyBilling.MonthlyBillingPhoneNumber")]
        public string MonthlyBillingPhoneNumber { get; set; }
    }
}