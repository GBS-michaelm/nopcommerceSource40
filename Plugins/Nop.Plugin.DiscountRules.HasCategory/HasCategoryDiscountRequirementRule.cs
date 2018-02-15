using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Catalog;

namespace Nop.Plugin.DiscountRules.HasCategory
{
    public partial class HasCategoryDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ISettingService _settingService;
        private readonly ICategoryService _categoryService;

        public HasCategoryDiscountRequirementRule(ISettingService settingService,
            ICategoryService categoryService)
        {
            this._settingService = settingService;
            this._categoryService = categoryService;
        }

        public int getCatId(int proId, int resCatId)
        {
            var productCategories = _categoryService.GetProductCategoriesByProductId(proId, true);
            if (productCategories.Count > 0)
            {
                var category = productCategories[0].Category;
                if (category != null)
                {
                    return category.Id;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            var restrictedCategoryIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedCategoryIds-{0}", request.DiscountRequirementId));
            if (String.IsNullOrWhiteSpace(restrictedCategoryIds))
            {
                //valid
                result.IsValid = true;
                return result;
            }

            if (request.Customer == null)
                return result;

            //we support three ways of specifying products:
            //1. The comma-separated list of product identifiers (e.g. 77, 123, 156).
            //2. The comma-separated list of product identifiers with quantities.
            //      {Product ID}:{Quantity}. For example, 77:1, 123:2, 156:3
            //3. The comma-separated list of product identifiers with quantity range.
            //      {Product ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
            var restrictedCategories = restrictedCategoryIds
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            if (!restrictedCategories.Any())
                return result;

            //group products in the cart by product ID
            //it could be the same product with distinct product attributes
            //that's why we get the total quantity of this product
            var cartQuery = from sci in request.Customer.ShoppingCartItems.LimitPerStore(request.Store.Id)
                            where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                            group sci by sci.ProductId into g
                            select new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
            var cart = cartQuery.ToList();

            bool allFound = true;
            foreach (var restrictedCategory in restrictedCategories)
            {
                if (String.IsNullOrWhiteSpace(restrictedCategory))
                    continue;

                bool found1 = false;
                foreach (var sci in cart)
                {
                    if (restrictedCategory.Contains(":"))
                    {
                        if (restrictedCategory.Contains("-"))
                        {
                            //the third way (the quantity rage specified)
                            //{Category ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
                            int restrictedCategoryId;
                            if (!int.TryParse(restrictedCategory.Split(new[] { ':' })[0], out restrictedCategoryId))
                                //parsing error; exit;
                                return result;
                            int quantityMin;
                            if (!int.TryParse(restrictedCategory.Split(new[] { ':' })[1].Split(new[] { '-' })[0], out quantityMin))
                                //parsing error; exit;
                                return result;
                            int quantityMax;
                            if (!int.TryParse(restrictedCategory.Split(new[] { ':' })[1].Split(new[] { '-' })[1], out quantityMax))
                                //parsing error; exit;
                                return result;

                            if (sci.ProductId == restrictedCategoryId && quantityMin <= sci.TotalQuantity && sci.TotalQuantity <= quantityMax)
                            {
                                found1 = true;
                                break;
                            }
                        }
                        else
                        {
                            //the second way (the quantity specified)
                            //{Category ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                            int restrictedCategoryId;
                            if (!int.TryParse(restrictedCategory.Split(new[] { ':' })[0], out restrictedCategoryId))
                                //parsing error; exit;
                                return result;
                            int quantity;
                            if (!int.TryParse(restrictedCategory.Split(new[] { ':' })[1], out quantity))
                                //parsing error; exit;
                                return result;

                            if (sci.ProductId == restrictedCategoryId && sci.TotalQuantity == quantity)
                            {
                                var testCatId = getCatId(sci.ProductId, restrictedCategoryId);

                                found1 = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //the first way (the quantity is not specified)
                        int restrictedCategoryId;
                        if (int.TryParse(restrictedCategory, out restrictedCategoryId))
                        {
                            if (sci.ProductId == restrictedCategoryId)
                            {
                                var testCatId = getCatId(sci.ProductId, restrictedCategoryId);

                                found1 = true;
                                break;
                            }
                        }
                    }
                }

                if (!found1)
                {
                    allFound = false;
                    break;
                }
            }

            if (allFound)
            {
                //valid
                result.IsValid = true;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesHasCategory/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories", "Restricted Categories [and quantity range]");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.Hint", "The comma-separated list of Category identifiers (e.g. 77, 123, 156). You can find a Category ID on its details page. You can also specify the comma-separated list of Category identifiers with quantities ({Category ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of Category identifiers with quantity range ({Category ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.AddNew", "Add Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.Choose", "Choose");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.Hint");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.AddNew");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasCategory.Fields.Categories.Choose");
            base.Uninstall();
        }
    }
}