using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Order.GBS.Components
{
	[ViewComponent(Name = "AddPhoneNumber")]
    public class AddPhoneNumberComponent : NopViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreContext _storeContext;

        public AddPhoneNumberComponent(IHttpContextAccessor httpContextAccessor,
            IStoreContext storeContext)
        {
            this._httpContextAccessor = httpContextAccessor;
            _storeContext = storeContext;
        }


        public IViewComponentResult Invoke()
        {
			return View("~/Plugins/Order.GBS/Views/OrderGBS/AddPhoneNumber.cshtml");
		}
    }
}
