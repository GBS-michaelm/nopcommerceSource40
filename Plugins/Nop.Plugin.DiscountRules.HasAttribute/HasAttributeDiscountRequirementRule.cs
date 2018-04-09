using System;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Data;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.DiscountRules.HasAttribute
{
    public partial class HasAttributeDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly ISettingService _settingService;

        public HasAttributeDiscountRequirementRule(ISettingService settingService,
            ICategoryService categoryService)
        {
            this._settingService = settingService;
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

            var RestrictedAttributeIds = _settingService.GetSettingByKey<string>(string.Format("DiscountRequirement.RestrictedAttributeIds-{0}", request.DiscountRequirementId));

            var settingId = _settingService.GetSettingById(2260);
            var settingInfo = _settingService.GetSetting(string.Format("DiscountRequirement.RestrictedAttributeIds-{0}", request.DiscountRequirementId));

            if (String.IsNullOrWhiteSpace(RestrictedAttributeIds))
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
            var restrictedAttributes = RestrictedAttributeIds
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            if (!restrictedAttributes.Any())
                return result;

            //group products in the cart by product ID
            //it could be the same product with distinct product attributes
            //that's why we get the total quantity of this product
            var cartQuery = from sci in request.Customer.ShoppingCartItems.LimitPerStore(request.Store.Id)
                            where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
                            group sci by sci.ProductId into g
                            select new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
            var cart = cartQuery.ToList();


            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            string select = "EXEC usp_getAttributeQty " + _workContext.CurrentCustomer.Id + "," + request.Store.Id + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);


            bool allFound = true;
            foreach (var restrictedAttribute in restrictedAttributes)
            {
                if (String.IsNullOrWhiteSpace(restrictedAttribute))
                    continue;

                bool found1 = false;
                //foreach (var sci in cart)
                foreach (DataRow dRow in dView.Table.Rows)
                {
                    if (restrictedAttribute.Contains(":"))
                    {
                        if (restrictedAttribute.Contains("-"))
                        {
                            //the third way (the quantity rage specified)
                            //{Attribute ID}:{Min quantity}-{Max quantity}. For example, 77:1-3, 123:2-5, 156:3-8
                            int restrictedAttributeId;
                            if (!int.TryParse(restrictedAttribute.Split(new[] { ':' })[0], out restrictedAttributeId))
                                //parsing error; exit;
                                return result;
                            int quantityMin;
                            if (!int.TryParse(restrictedAttribute.Split(new[] { ':' })[1].Split(new[] { '-' })[0], out quantityMin))
                                //parsing error; exit;
                                return result;
                            int quantityMax;
                            if (!int.TryParse(restrictedAttribute.Split(new[] { ':' })[1].Split(new[] { '-' })[1], out quantityMax))
                                //parsing error; exit;
                                return result;

                            //if (sci.ProductId == restrictedAttributeId && quantityMin <= sci.TotalQuantity && sci.TotalQuantity <= quantityMax)
                            if ((int)dRow["SpecificationAttributeOptionId"] == restrictedAttributeId && quantityMin <= (int)dRow["TotalQuantity"] && (int)dRow["TotalQuantity"] <= quantityMax)
                            {
                                found1 = true;
                                break;
                            }
                        }
                        else
                        {
                            //the second way (the quantity specified)
                            //{Attribute ID}:{Quantity}. For example, 77:1, 123:2, 156:3
                            int restrictedAttributeId;
                            if (!int.TryParse(restrictedAttribute.Split(new[] { ':' })[0], out restrictedAttributeId))
                                //parsing error; exit;
                                return result;
                            int quantity;
                            if (!int.TryParse(restrictedAttribute.Split(new[] { ':' })[1], out quantity))
                                //parsing error; exit;
                                return result;

                            if ((int)dRow["SpecificationAttributeOptionId"] == restrictedAttributeId && (int)dRow["TotalQuantity"] >= quantity)
                            {
                                found1 = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //the first way (the quantity is not specified)
                        int restrictedAttributeId;
                        if (int.TryParse(restrictedAttribute, out restrictedAttributeId))
                        {
                            if ((int)dRow["SpecificationAttributeOptionId"] == restrictedAttributeId)
                            {
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
            string result = "Plugins/DiscountRulesHasAttribute/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes", "Restricted Attributes [and quantity range]");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.Hint", "The comma-separated list of Attribute identifiers (e.g. 77, 123, 156). You can find a Attribute ID on its details page. You can also specify the comma-separated list of Attribute identifiers with quantities ({Attribute ID}:{Quantity}. for example, 77:1, 123:2, 156:3). And you can also specify the comma-separated list of Attribute identifiers with quantity range ({Attribute ID}:{Min quantity}-{Max quantity}. for example, 77:1-3, 123:2-5, 156:3-8).");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.AddNew", "Add Attribute");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.Choose", "Choose");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.Hint");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.AddNew");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.HasAttribute.Fields.Attributes.Choose");
            base.Uninstall();
        }
    }
}