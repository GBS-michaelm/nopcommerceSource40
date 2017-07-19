using Nop.Plugin.Widgets.CustomersCanvas.Controllers;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.CustomersCanvas
{
    public static class Utils
    {
        public class FakeController : BasePluginController
        {
        }

        public static string ToFsCompatibleName(string name)
        {
            var pathInvalidChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars());

            return new string(name.Select(c => pathInvalidChars.Contains(c) ? '-' : c).ToArray());
        }

        public static IHtmlString ToJsString<TModel>(this HtmlHelper<TModel> helper, string str)
        {
            return helper.Raw(string.Format("'{0}'", HttpUtility.JavaScriptStringEncode(str)));
        }

        public static string ReverseMapPath(string path, string removePath)
        {
            string res = string.Format("{0}", path.Replace(removePath, "").Replace("\\", "/"));
            if (res[0] == '/')
            {
                return res.TrimStart('/');
            }
            return res;
        }

        public static string GetRazorViewAsString(object model, string filePath)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(System.Web.HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new FakeController());
            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }
    }
}
