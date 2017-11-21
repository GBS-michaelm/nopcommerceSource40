using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Payments.PurchaseOrder.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.PurchaseOrderNumber")]
        //[AllowHtml]
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderName { get; set; }
        public string PurchaseOrderPhoneNumber { get; set; }
    }
}