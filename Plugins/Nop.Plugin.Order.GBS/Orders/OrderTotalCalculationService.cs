using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Discounts.Cache;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Core.Caching;
using Nop.Core.Plugins;
using Nop.Plugin.DataAccess.GBS;
using System.Data;

namespace Nop.Plugin.Order.GBS.Orders
{
    public class GBSOrderTotalCalculationService : OrderTotalCalculationService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDiscountService _discountService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;

        public GBSOrderTotalCalculationService(IWorkContext workContext,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IShippingService shippingService,
            IPaymentService paymentService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IDiscountService discountService,
            IGiftCardService giftCardService,
            IGenericAttributeService genericAttributeService,
            IRewardPointService rewardPointService,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            IPluginFinder pluginFinder) : base (workContext,
            storeContext,
            priceCalculationService,
            taxService,
            shippingService,
            paymentService,
            checkoutAttributeParser,
            discountService,
            giftCardService,
            genericAttributeService,
            rewardPointService,
            taxSettings,
            rewardPointsSettings,
            shippingSettings,
            shoppingCartSettings,
            catalogSettings)
        {
            this._cacheManager = cacheManager;
            this._discountService = discountService;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
        }

        protected IList<DiscountService.DiscountRequirementForCaching> GetReqirementsForCaching(IEnumerable<DiscountRequirement> requirements)
        {
            var requirementForCaching = requirements.Select(requirement => new DiscountService.DiscountRequirementForCaching
            {
                Id = requirement.Id,
                IsGroup = requirement.IsGroup,
                SystemName = requirement.DiscountRequirementRuleSystemName,
                InteractionType = requirement.InteractionType,
                ChildRequirements = GetReqirementsForCaching(requirement.ChildRequirements)
            });

            return requirementForCaching.ToList();
        }

        protected override decimal GetOrderTotalDiscount(Customer customer, decimal orderTotal, out List<DiscountForCaching> appliedDiscounts)
        {
            var baseDiscount = base.GetOrderTotalDiscount(customer, orderTotal, out appliedDiscounts);

            List<decimal> discountValues = new List<decimal>();

            foreach (var discount in appliedDiscounts)
            {
                //discount requirements
                string key = string.Format(DiscountEventConsumer.DISCOUNT_REQUIREMENT_MODEL_KEY, discount.Id);
                var requirementsForCaching = _cacheManager.Get(key, () =>
                {
                    var requirements = _discountService.GetAllDiscountRequirements(discount.Id, true);
                    return GetReqirementsForCaching(requirements);
                });

                var topLevelGroup = requirementsForCaching.FirstOrDefault();

                List<int> discountIds = new List<int>();

                if (topLevelGroup != null)
                {
                    GetDiscountAmount(requirementsForCaching, topLevelGroup.InteractionType.Value, customer, topLevelGroup, discount, ref discountIds);
                }
                

                foreach (var Id in discountIds)
                {
                    //Go get price for the disocunt requirement
                    DBManager dbmanager = new DBManager();
                    Dictionary<string, string> paramDic = new Dictionary<string, string>();
                    string select = "EXEC usp_getDiscountAmount " + Id + "";
                    DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

                    if (dView.Count > 0)
                    {
                        foreach (DataRow dRow in dView.Table.Rows)
                        {
                            discountValues.Add((decimal)dRow["DiscountPrice"]);
                        }
                    }

                    //discountValues.Add();
                }
            }

            if (discountValues.Count > 0)
            {
                baseDiscount = discountValues.Max();
            }

            return baseDiscount;
        }

        protected bool GetDiscountAmount(IEnumerable<DiscountService.DiscountRequirementForCaching> requirements,
            RequirementGroupInteractionType groupInteractionType, Customer customer, DiscountService.DiscountRequirementForCaching topLevelGroup, DiscountForCaching discount, ref List<int> discountIds)
        {
            var result = false;

            foreach (var requirement in requirements)
            {
                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    var interactionType = requirement.InteractionType.HasValue
                        ? requirement.InteractionType.Value : RequirementGroupInteractionType.And;
                    result = GetDiscountAmount(requirement.ChildRequirements, interactionType, customer, topLevelGroup, discount, ref discountIds);

                    //add discount id
                    var requirementChild = _discountService.GetAllDiscountRequirements(discount.Id).ToList().Where(x => x.Id == requirement.Id);

                    if (result && requirementChild.Count() > 0)
                        discountIds.Add(requirement.Id);
                }
                else
                {
                    //or try to get validation result for the requirement
                    var requirementRulePlugin = _discountService.LoadDiscountRequirementRuleBySystemName(requirement.SystemName);
                    if (requirementRulePlugin == null)
                        continue;

                    if (!_pluginFinder.AuthorizedForUser(requirementRulePlugin.PluginDescriptor, customer))
                        continue;

                    if (!_pluginFinder.AuthenticateStore(requirementRulePlugin.PluginDescriptor, _storeContext.CurrentStore.Id))
                        continue;

                    var ruleResult = requirementRulePlugin.CheckRequirement(new DiscountRequirementValidationRequest
                    {
                        DiscountRequirementId = requirement.Id,
                        Customer = customer,
                        Store = _storeContext.CurrentStore
                    });

                    result = ruleResult.IsValid;
                }

                //all requirements must be met, so return false
                if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                    return result;

                //any of requirements must be met, so return true
                if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                    return result;
            }

            return result;
        }
    }
}
