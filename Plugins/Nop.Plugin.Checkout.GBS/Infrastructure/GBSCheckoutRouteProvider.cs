using System;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Infrastructure;
using Nop.Web.Framework.Localization;


namespace Nop.Plugin.Checkout.GBS.Infrastructure
{
    public partial class GBSCheckoutRouteProvider : IRouteProvider
    {
        //RouteProvider _baseRouteProvider;
        public GBSCheckoutRouteProvider()
        {
            //_baseRouteProvider = new RouteProvider();
        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            ////register base NopCommerce routes since we are overriding this method
            //_baseRouteProvider.RegisterRoutes(routes);

            ////register again routes we want to change from base


            //routes.Remove(routes["Checkout"]);

            routes.MapLocalizedRoute("GBSShoppingCartPA",
                            "shoppingcart/productdetails_attributechange",
                            new { controller = "GBSShoppingCart", action = "MyPDA" },
                            new[] { "Nop.Plugin.ShoppingCart.GBS.Controllers" });
            //routes.MapLocalizedRoute("GBSOrderSummary",
            //             "ordersummary/",
            //             new { controller = "GBSShoppingCart", action = "OrderSummary" },
            //             new[] { "Nop.Plugin.ShoppingCart.GBS.Controllers" });

            //checkout pages
            routes.MapLocalizedRoute("GBSCheckout",
                            "checkout/",
                            new { controller = "Checkout", action = "Index" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSNewShipping",
                            "checkout/shippingaddress",
                            new { controller = "Checkout", action = "ShippingAddress" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSSelectShippingAddress",
                            "checkout/selectshippingaddress",
                            new { controller = "Checkout", action = "SelectShippingAddress" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "Checkout", action = "PaymentInfo" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            //routes.MapLocalizedRoute("GBSCart",
            //                "cart/",
            //                new { controller = "ShoppingCart", action = "Cart" },
            //                new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            //routes.MapLocalizedRoute("CheckoutOnePage",
            //                "onepagecheckout/",
            //                new { controller = "Checkout", action = "OnePageCheckout" },
            //                new[] { "Nop.Web.Controllers" });
            //routes.MapLocalizedRoute("CheckoutShippingAddress",
            //                "checkout/shippingaddress",
            //                new { controller = "Checkout", action = "ShippingAddress" },
            //                new[] { "Nop.Web.Controllers" });
            //routes.MapLocalizedRoute("CheckoutSelectShippingAddress",
            //                "checkout/selectshippingaddress",
            //                new { controller = "Checkout", action = "SelectShippingAddress" },
            //                new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("GBSCheckoutBillingAddress",
                            "checkout/billingaddress",
                            new { controller = "Checkout", action = "BillingAddress" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            //routes.MapLocalizedRoute("GBSCheckoutSelectBillingAddress",
            //                "checkout/selectbillingaddress",
            //                new { controller = "Checkout", action = "SelectBillingAddress" },
            //                new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            //routes.MapLocalizedRoute("CheckoutShippingMethod",
            //                "checkout/shippingmethod",
            //                new { controller = "Checkout", action = "ShippingMethod" },
            //                new[] { "Nop.Web.Controllers" });
            //routes.MapLocalizedRoute("CheckoutPaymentMethod",
            //                "checkout/paymentmethod",
            //                new { controller = "Checkout", action = "PaymentMethod" },
            //                new[] { "Nop.Web.Controllers" });
            //routes.MapLocalizedRoute("CheckoutPaymentInfo",
            //                "checkout/paymentinfo",
            //                new { controller = "Checkout", action = "PaymentInfo" },
            //                new[] { "Nop.Web.Controllers" });
            //routes.MapLocalizedRoute("GBSCheckoutConfirm",
            //                "checkout/confirm",
            //                new { controller = "Checkout", action = "Confirm" },
            //              new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            //routes.MapLocalizedRoute("CheckoutCompleted",
            //                "checkout/completed/{orderId}",
            //                new { controller = "Checkout", action = "Completed", orderId = UrlParameter.Optional },
            //                new { orderId = @"\d+" },
            //                new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("GBSCheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "Checkout", action = "Confirm" },
                            new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSApplyDiscount",
                           "gbsapplydiscount/",
                           new { controller = "Checkout", action = "ApplyDiscount" },
                           new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSRemoveDiscount",
                           "gbsremovediscount/",
                           new { controller = "Checkout", action = "RemoveDiscount" },
                           new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
            routes.MapLocalizedRoute("GBSEstimateShippingTotal",
                           "estimateshippingtotal/",
                           new { controller = "Checkout", action = "EstimateShippingTotal" },
                           new[] { "Nop.Plugin.Checkout.GBS.Controllers" });
           



        }
    }
}
