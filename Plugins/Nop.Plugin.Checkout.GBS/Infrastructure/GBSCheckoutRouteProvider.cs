using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

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
                return int.MaxValue;
            }
        }

        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute("BuyItAgain",
                            "buyitagain/{orderItemId}/{isLegacy}",
                            new { controller = "GBSShoppingCart", action = "BuyItAgain" },
                            new { orderItemId = @"\d+", isLegacy = @"^(true|false)$" });

            ////register base NopCommerce routes since we are overriding this method
            //_baseRouteProvider.RegisterRoutes(routes);

            ////register again routes we want to change from base


            //routes.Remove(routes["Checkout"]);

            routes.MapRoute("GBSShoppingCartPA",
                            "shoppingcart/productdetails_attributechange",
                            new { controller = "GBSShoppingCart", action = "MyPDA" });
                        
            routes.MapRoute("GBSAddProductToCart-Details",
                            "addproducttocart/details/{productId}/{shoppingCartTypeId}",
                            new { controller = "GBSShoppingCart", action = "AddProductToCart_Details" },
                            new { productId = @"\d+", shoppingCartTypeId = @"\d+" });

            routes.MapRoute("GBSAddIframeNameBadgeToCart",
                            "addiframenamebadgetocart",
                            new { controller = "GBSShoppingCart", action = "NameBadgeIframeAddToCart" });

            routes.MapRoute("GBSAddProductToCart-Amalgamation",
                            "addproducttocart/amalgamation/{productId}/{shoppingCartTypeId}/{quantity}",
                            new { controller = "GBSShoppingCart", action = "AddProductToCart_Amalgamation" },
                            new { productId = @"\d+", shoppingCartTypeId = @"\d+", quantity = @"\d+" });

            routes.MapRoute("GBS-AmalgamationCartCategoryTotal",
                            "amalgamationgetcarttotal/{categoryId}/{productId}",
                            new { controller = "GBSShoppingCart", action = "AmalgamationCartCategoryTotal" },
                            new { categoryId = @"\d+", productId = @"\d+" });

            routes.MapRoute("GBS-AmalgamationBar",
                            "amalgamationbar/{categoryId}/{featuredProductId}",
                            new { controller = "GBSShoppingCart", action = "AmalgamationBarUpdate" },
                            new { categoryId = @"\d+", featuredProductId = @"\d+" });

            //routes.MapLocalizedRoute("GBSOrderSummary",
            //             "ordersummary/",
            //             new { controller = "GBSShoppingCart", action = "OrderSummary" },
            //             new[] { "Nop.Plugin.ShoppingCart.GBS.Controllers" });

            //checkout pages
            routes.MapRoute("GBSCheckout",
                            "checkout/",
                            new { controller = "GBSCheckout", action = "Index" });

            routes.MapRoute("GBSNewShipping",
                            "checkout/shippingaddress",
                            new { controller = "GBSCheckout", action = "ShippingAddress" });

            routes.MapRoute("GBSSelectShippingAddress",
                            "checkout/selectshippingaddress",
                            new { controller = "GBSCheckout", action = "SelectShippingAddress" });

            routes.MapRoute("GBSPaymentInfo",
                            "checkout/paymentinfo",
                            new { controller = "GBSCheckout", action = "PaymentInfo" });

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
            routes.MapRoute("GBSCheckoutBillingAddress",
                            "checkout/billingaddress",
                            new { controller = "GBSCheckout", action = "BillingAddress" });

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

            routes.MapRoute("GBSCheckoutConfirm",
                            "checkout/confirm",
                            new { controller = "GBSCheckout", action = "Confirm" });

            routes.MapRoute("GBSApplyDiscount",
                           "gbsapplydiscount/",
                           new { controller = "GBSCheckout", action = "ApplyDiscount" });

            routes.MapRoute("GBSRemoveDiscount",
                           "gbsremovediscount/",
                           new { controller = "GBSCheckout", action = "RemoveDiscount" });

            routes.MapRoute("GBSEstimateShippingTotal",
                           "estimateshippingtotal/",
                           new { controller = "GBSCheckout", action = "EstimateShippingTotal" });

            routes.MapRoute("GBSSubmitItem",
                           "shoppingcart/submititem",
                           new { controller = "GBSShoppingCart", action = "SubmitItem" });

            routes.MapRoute("GBSSetProductOptions",
                          "shoppingcart/setproductoptions",
                          new { controller = "GBSShoppingCart", action = "SetProductOptions" });


        }
    }
}
