using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Payments.GBS.PurchaseOrder.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderName")]
        public string PurchaseOrderName { get; set; }

        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderPhoneNumber")]
        public string PurchaseOrderPhoneNumber { get; set; }
    }
}