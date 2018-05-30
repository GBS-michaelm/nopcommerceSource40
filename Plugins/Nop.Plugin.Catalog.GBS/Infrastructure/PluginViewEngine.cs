using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Catalog.GBS.Infrastructure
{
    public class GBSCatalogViewEngine : ThemeableRazorViewEngine
    {
        public GBSCatalogViewEngine()
        {
            AddViewLocationFormats();
        }

        public void AddViewLocationFormats()
        {
            var additionalViewLocationFormats = new List<string>()
              {
                //"~/Plugins/Catalog.GBS/Views/{0}.cshtml",
                //"~/Plugins/Catalog.GBS/Views/{1}/{0}.cshtml",

              };

            var currentViewLocationFormats = ViewLocationFormats.ToList();

            currentViewLocationFormats.InsertRange(0, additionalViewLocationFormats);

            ViewLocationFormats = currentViewLocationFormats.ToArray();

            var currentPartialViewLocationFormats = PartialViewLocationFormats.ToList();

            currentPartialViewLocationFormats.InsertRange(0, additionalViewLocationFormats);

            PartialViewLocationFormats = currentPartialViewLocationFormats.ToArray();
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            ViewEngineResult viewEngineResult = base.FindPartialView(controllerContext, partialViewName, useCache);

            if (useCache && viewEngineResult.View == null)
            {
                viewEngineResult = base.FindPartialView(controllerContext, partialViewName, false);
            }

            return viewEngineResult;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            ViewEngineResult viewEngineResult = base.FindView(controllerContext, viewName, masterName, useCache);

            if (useCache && viewEngineResult.View == null)
            {
                viewEngineResult = base.FindView(controllerContext, viewName, masterName, false);
            }

            return viewEngineResult;
        }



    }
}