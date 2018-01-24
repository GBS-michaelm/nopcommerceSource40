using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class CustomViewEngine : ThemeableRazorViewEngine
    {
        public CustomViewEngine()
        {
            PartialViewLocationFormats = new[] {
                "~/Plugins/Widgets.CustomersCanvas/Views/{0}.cshtml" ,
                "~/Plugins/Widgets.CustomersCanvas/Views/{1}/{0}.cshtml" };
            ViewLocationFormats = new[] {
                "~/Plugins/Widgets.CustomersCanvas/Views/{0}.cshtml",
                "~/Plugins/Widgets.CustomersCanvas/Views/{1}/{0}.cshtml"};
        }
    }
}
