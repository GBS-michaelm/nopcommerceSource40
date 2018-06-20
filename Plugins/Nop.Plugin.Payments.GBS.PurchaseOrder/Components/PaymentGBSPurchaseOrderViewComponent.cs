using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.GBS.PurchaseOrder.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.GBS.PurchaseOrder.Components
{
    [ViewComponent(Name = "PaymentGBSPurchaseOrder")]
    public class PaymentGBSPurchaseOrderViewComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentGBSPurchaseOrderViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }


        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();
            var form = this.Request.Form;
            model.PurchaseOrderNumber = form["PurchaseOrderNumber"];
            model.PurchaseOrderName = form["PurchaseOrderName"];
            model.PurchaseOrderPhoneNumber = form["PurchaseOrderPhoneNumber"];

            if (this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderNumber") != null)
            {
                model.PurchaseOrderNumber = this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderNumber");
            }
            if (this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderName") != null)
            {
                model.PurchaseOrderName = this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderName");
            }
            if (this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderPhoneNumber") != null)
            {
                model.PurchaseOrderPhoneNumber = this._httpContextAccessor.HttpContext.Session.GetString("purchaseOrderPhoneNumber");
            }                     

            return View("~/Plugins/Payments.GBS.PurchaseOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}
