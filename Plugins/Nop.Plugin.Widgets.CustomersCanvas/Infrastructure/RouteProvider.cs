using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 1;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            ViewEngines.Engines.Insert(0, new CustomViewEngine());
            var orderRoute = routes.MapRoute("Plugin.Widgets.CustomersCanvas.Orders", "Admin/Order/List",
                     new { controller = "CcOrder", action = "List", area = "Admin" },
                     new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" }
            );
            routes.Remove(orderRoute);
            routes.Insert(0, orderRoute);


            // todo change area to aurigma
            #region configure
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.Configure",
                "Plugins/CcConfig/Configure",
                new { controller = "CcConfig", action = "Configure" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region editor page
            routes.MapLocalizedRoute("Plugins.Widgets.CustomersCanvas.CcWidget.EditorPage",
                "Plugins/CcWidget/EditorPage",
                new { controller = "CcWidget", action = "EditorPage" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region updateFonts
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcConfig.ReloadLocalFonts",
               "Plugins/CcConfig/ReloadLocalFonts",
               new { controller = "CcConfig", action = "ReloadLocalFonts" },
               new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region product tab
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.ProductAdminTab",
                "Plugins/CcProduct/ProductAdminTab",
                new { controller = "CcProduct", action = "ProductAdminTab" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region save\delete attr
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.SaveCcAttributes",
                "Plugins/CcProduct/SaveCcAttributes",
                new { controller = "CcProduct", action = "SaveCcAttributes" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.SaveCcAttribute",
                "Plugins/CcProduct/SaveCcAttribute",
                new { controller = "CcProduct", action = "SaveCcAttribute" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.DeleteCcAttributes",
                "Plugins/CcProduct/DeleteCcAttributes",
                new { controller = "CcProduct", action = "DeleteCcAttributes" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.GetSpecAttributes",
                "Plugins/CcProduct/GetSpecAttributes",
                new { controller = "CcProduct", action = "GetSpecAttributes" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.DeleteSpecAttribute",
                "Plugins/CcProduct/DeleteSpecAttribute",
                new { controller = "CcProduct", action = "DeleteSpecAttribute" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            #endregion

            #region update\delete editor\config
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcEditor.Index",
                "Plugins/CcEditor/Index",
                new { controller = "CcEditor", action = "Index" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcEditor.DeleteEditor",
                "Plugins/CcEditor/DeleteEditor",
                new { controller = "CcEditor", action = "DeleteEditor" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcEditor.UploadEditor",
                "Plugins/CcEditor/UploadEditor",
                new { controller = "CcEditor", action = "UploadEditor" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcEditor.DeleteConfig",
                "Plugins/CcEditor/DeleteConfig",
                new { controller = "CcEditor", action = "DeleteConfig" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcEditor.UploadConfig",
                "Plugins/CcEditor/UploadConfig",
                new { controller = "CcEditor", action = "UploadConfig" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region update\submit items
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcWidget.UpdateItem",
                "Plugins/CcWidget/UpdateItem",
                new { controller = "CcWidget", action = "UpdateItem" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcWidget.SubmitItem",
                "Plugins/CcWidget/SubmitItem",
                new { controller = "CcWidget", action = "SubmitItem" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region GetCartItemsData
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcWidget.GetCartItemsData",
                "Plugins/CcWidget/GetCartItemsData",
                new { controller = "CcWidget", action = "GetCartItemsData" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });
            #endregion

            #region color\image attribute
            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.ProductAttributeValueCreatePopup",
                "Plugins/CcProduct/ProductAttributeValueCreatePopup",
                new { controller = "CcProduct", action = "ProductAttributeValueCreatePopup" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.ProductAttributeValueEditPopup",
                "Plugins/CcProduct/ProductAttributeValueEditPopup",
                new { controller = "CcProduct", action = "ProductAttributeValueEditPopup" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.ProductAttributeValueList",
                "Plugins/CcProduct/ProductAttributeValueList",
                new { controller = "CcProduct", action = "ProductAttributeValueList" },
                new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });

            routes.MapRoute("Plugins.Widgets.CustomersCanvas.CcProduct.AsyncUpload",
               "Plugins/CcProduct/AsyncUpload",
               new { controller = "CcProduct", action = "AsyncUpload" },
               new[] { "Nop.Plugin.Widgets.CustomersCanvas.Controllers" });


            #endregion

        }
    }
}