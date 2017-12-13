using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.GBS.MonthlyBilling.MonthlyBillingReference")]
        [AllowHtml]
        public string MonthlyBillingReference { get; set; }

        [NopResourceDisplayName("Plugins.Payment.GBS.MonthlyBilling.MonthlyBillingName")]
        [AllowHtml]
        public string MonthlyBillingName { get; set; }

        [NopResourceDisplayName("Plugins.Payment.GBS.MonthlyBilling.MonthlyBillingPhoneNumber")]
        [AllowHtml]
        public string MonthlyBillingPhoneNumber { get; set; }
    }
}