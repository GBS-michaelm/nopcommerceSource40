using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;
using Nop.Plugin.Catalog.GBS.Infrastructure;
using Nop.Web.Framework.Seo;

namespace Nop.Plugin.Catalog.GBS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapGenericPathRoute("CategoryGenericUrl",
            //                           "{generic_se_name}",
            //                           new { controller = "Catalog", action = "Category" },
            //                           new[] { "Nop.Plugin.Catalog.GBS.Controllers" });


            //  ViewEngines.Engines.Insert(0, new GBSCatalogViewEngine());

            //product admin
            routes.MapLocalizedRoute("SportsTeamsList",
                            "SportsTeamsList",
                            new { controller = "GBSCatalog", action = "SportsTeamsList" },
                            new[] { "Nop.Plugin.Catalog.GBS.Controllers" });

            var adminProductListroute = routes.MapRoute("List",
            "Admin/Product/List",
            new { controller = "Product", action = "List" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );

            adminProductListroute.DataTokens.Add("area", "admin");
            routes.Remove(adminProductListroute);
            routes.Insert(0, adminProductListroute);

            var adminProductCreateroute = routes.MapRoute("Create",
            "Admin/Product/Create",
            new { controller = "Product", action = "Create" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminProductCreateroute.DataTokens.Add("area", "admin");
            routes.Remove(adminProductCreateroute);
            routes.Insert(0, adminProductCreateroute);

            var adminProductEditroute = routes.MapRoute("Edit",
            "Admin/Product/Edit/{Id}",
            new { controller = "Product", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminProductEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminProductEditroute);
            routes.Insert(0, adminProductEditroute);

            var adminRequiredProductAddPopuproute = routes.MapRoute("RequiredProductAddPopup",
            "Admin/Product/RequiredProductAddPopup",
            new { controller = "Product", action = "RequiredProductAddPopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminRequiredProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminRequiredProductAddPopuproute);
            routes.Insert(0, adminRequiredProductAddPopuproute);

            var adminRelatedProductAddPopuproute = routes.MapRoute("RelatedProductAddPopup",
            "Admin/Product/RelatedProductAddPopup",
            new { controller = "Product", action = "RelatedProductAddPopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminRelatedProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminRelatedProductAddPopuproute);
            routes.Insert(0, adminRelatedProductAddPopuproute);

            var adminCrossSellProductAddPopuproute = routes.MapRoute("CrossSellProductAddPopup",
            "Admin/Product/CrossSellProductAddPopup",
            new { controller = "Product", action = "CrossSellProductAddPopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminCrossSellProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminCrossSellProductAddPopuproute);
            routes.Insert(0, adminCrossSellProductAddPopuproute);

            var adminAssociatedProductAddPopuproute = routes.MapRoute("AssociatedProductAddPopup",
            "Admin/Product/AssociatedProductAddPopup",
            new { controller = "Product", action = "AssociatedProductAddPopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminAssociatedProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminAssociatedProductAddPopuproute);
            routes.Insert(0, adminAssociatedProductAddPopuproute);

            var adminBulkEditroute = routes.MapRoute("BulkEdit",
            "Admin/Product/BulkEdit",
            new { controller = "Product", action = "BulkEdit" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminBulkEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminBulkEditroute);
            routes.Insert(0, adminBulkEditroute);

            var adminAssociateProductToAttributeValuePopuproute = routes.MapRoute("AssociateProductToAttributeValuePopup",
            "Admin/Product/AssociateProductToAttributeValuePopup",
            new { controller = "Product", action = "AssociateProductToAttributeValuePopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminAssociateProductToAttributeValuePopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminAssociateProductToAttributeValuePopuproute);
            routes.Insert(0, adminAssociateProductToAttributeValuePopuproute);


            //Category Admin
            var adminProductAddPopuproute = routes.MapRoute("ProductAddPopup",
            "Admin/Category/ProductAddPopup/{Id}",
            new { controller = "Category", action = "ProductAddPopup" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminProductAddPopuproute);
            routes.Insert(0, adminProductAddPopuproute);

            var adminCategoryEditroute = routes.MapRoute("Edit",
            "Admin/Category/Edit/{Id}",
            new { controller = "Category", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminCategoryEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminCategoryEditroute);
            routes.Insert(0, adminCategoryEditroute);

            var adminCategoryCreateroute = routes.MapRoute("Create",
            "Admin/Category/Create",
            new { controller = "Category", action = "Create" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminCategoryCreateroute.DataTokens.Add("area", "admin");
            routes.Remove(adminCategoryCreateroute);
            routes.Insert(0, adminCategoryCreateroute);

            //Customer Role Admin

            var adminAssociateProductToCustomerRolePopuproute = routes.MapRoute("AssociateProductToCustomerRolePopup",
            "Admin/CustomerRole/AssociateProductToCustomerRolePopup",
            new { controller = "CustomerRole", action = "AssociateProductToCustomerRolePopup" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminAssociateProductToCustomerRolePopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminAssociateProductToCustomerRolePopuproute);
            routes.Insert(0, adminAssociateProductToCustomerRolePopuproute);
        
            var adminCustomerRoleEditroute = routes.MapRoute("Edit",
            "Admin/CustomerRole/Edit/{Id}",
            new { controller = "CustomerRole", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminCustomerRoleEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminCustomerRoleEditroute);
            routes.Insert(0, adminCustomerRoleEditroute);

            var adminCustomerRoleCreateroute = routes.MapRoute("Create",
            "Admin/CustomerRole/Create",
            new { controller = "CustomerRole", action = "Create" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminCustomerRoleCreateroute.DataTokens.Add("area", "admin");
            routes.Remove(adminCustomerRoleCreateroute);
            routes.Insert(0, adminCustomerRoleCreateroute);


            //Discount Admin
            var adminDiscountProductAddPopuproute = routes.MapRoute("ProductAddPopup",
            "Admin/Discount/ProductAddPopup/{Id}",
            new { controller = "Discount", action = "ProductAddPopup" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminDiscountProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminDiscountProductAddPopuproute);
            routes.Insert(0, adminDiscountProductAddPopuproute);

            var adminDiscountEditroute = routes.MapRoute("Edit",
            "Admin/Discount/Edit/{Id}",
            new { controller = "Discount", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminDiscountEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminDiscountEditroute);
            routes.Insert(0, adminDiscountEditroute);

            var adminDiscountCreateroute = routes.MapRoute("Create",
            "Admin/Discount/Create",
            new { controller = "Discount", action = "Create" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminDiscountCreateroute.DataTokens.Add("area", "admin");
            routes.Remove(adminDiscountCreateroute);
            routes.Insert(0, adminDiscountCreateroute);

            //Manufacturer Admin
            var adminManufacturerProductAddPopuproute = routes.MapRoute("ProductAddPopup",
            "Admin/Manufacturer/ProductAddPopup/{Id}",
            new { controller = "Manufacturer", action = "ProductAddPopup" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminManufacturerProductAddPopuproute.DataTokens.Add("area", "admin");
            routes.Remove(adminManufacturerProductAddPopuproute);
            routes.Insert(0, adminManufacturerProductAddPopuproute);

            var adminManufacturerEditroute = routes.MapRoute("Edit",
            "Admin/Manufacturer/Edit/{Id}",
            new { controller = "Manufacturer", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminManufacturerEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminManufacturerEditroute);
            routes.Insert(0, adminManufacturerEditroute);

            var adminManufacturerCreateroute = routes.MapRoute("Create",
            "Admin/Manufacturer/Create",
            new { controller = "Manufacturer", action = "Create" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminManufacturerCreateroute.DataTokens.Add("area", "admin");
            routes.Remove(adminManufacturerCreateroute);
            routes.Insert(0, adminManufacturerCreateroute);


            //Order Admin
            var adminAddProductToOrderroute = routes.MapRoute("AddProductToOrder",
            "Admin/Order/AddProductToOrder",
            new { controller = "Order", action = "AddProductToOrder" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminAddProductToOrderroute.DataTokens.Add("area", "admin");
            routes.Remove(adminAddProductToOrderroute);
            routes.Insert(0, adminAddProductToOrderroute);

            var adminBestsellersReportroute = routes.MapRoute("BestsellersReport",
            "Admin/Order/BestsellersReport",
            new { controller = "Order", action = "BestsellersReport" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminBestsellersReportroute.DataTokens.Add("area", "admin");
            routes.Remove(adminBestsellersReportroute);
            routes.Insert(0, adminBestsellersReportroute);

            var adminNeverSoldReportroute = routes.MapRoute("NeverSoldReport",
            "Admin/Order/NeverSoldReport",
            new { controller = "Order", action = "NeverSoldReport" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminNeverSoldReportroute.DataTokens.Add("area", "admin");
            routes.Remove(adminNeverSoldReportroute);
            routes.Insert(0, adminNeverSoldReportroute);

            var adminOrderEditroute = routes.MapRoute("Edit",
            "Admin/Order/Edit/{Id}",
            new { controller = "Order", action = "Edit" },
            new { Id = @"\d+" },
            new[] { "Nop.Plugin.Catalog.GBS.Controllers" }
            );
            adminOrderEditroute.DataTokens.Add("area", "admin");
            routes.Remove(adminOrderEditroute);
            routes.Insert(0, adminOrderEditroute);



        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }
    }
}
