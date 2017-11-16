using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GBS.PurchaseOrder.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderNumber")]
        [AllowHtml]
        public string PurchaseOrderNumber { get; set; }

        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderName")]
        [AllowHtml]
        public string PurchaseOrderName { get; set; }

        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderPhoneNumber")]
        [AllowHtml]
        public string PurchaseOrderPhoneNumber { get; set; }
    }
}