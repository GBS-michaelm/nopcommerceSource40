using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.GBS.MonthlyBilling.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.GBS.MonthlyBilling.Components
{
    [ViewComponent(Name = "PaymentGBSMonthlyBilling")]
    public class PaymentGBSMonthlyBillingComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentGBSMonthlyBillingComponent(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }


        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel();
            var form = this.Request.Form;
            model.MonthlyBillingName = form["MonthlyBillingName"];
            model.MonthlyBillingPhoneNumber = form["MonthlyBillingPhoneNumber"];
            model.MonthlyBillingReference = form["MonthlyBillingReference"];

            if (this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingName") != null)
            {
                model.MonthlyBillingName = this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingName");
            }
            if (this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingPhoneNumber") != null)
            {
                model.MonthlyBillingPhoneNumber = this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingPhoneNumber");
            }
            if (this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingReference") != null)
            {
                model.MonthlyBillingReference = this._httpContextAccessor.HttpContext.Session.GetString("monthlyBillingReference");
            }                     

            return View("~/Plugins/Payments.GBS.PurchaseOrder/Views/PaymentInfo.cshtml", model);
        }
    }
}
