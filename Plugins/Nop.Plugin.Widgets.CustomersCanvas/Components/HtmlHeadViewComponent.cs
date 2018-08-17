using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Components
{
    [ViewComponent(Name = "HtmlHead")]
    public class HtmlHeadViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return HeadHtmlTag();
        }

        private IViewComponentResult HeadHtmlTag()
        {
            return View("~/Plugins/Widgets.CustomersCanvas/Views/_Head.cshtml");
        }

    }
}
