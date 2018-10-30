using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Linq;

namespace Nop.Plugin.DiscountRules.ProductQuantity
{
	public partial class ProductQuantityDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
	{
		#region Fields

		private readonly IActionContextAccessor _actionContextAccessor;
		private readonly IDiscountService _discountService;
		private readonly ISettingService _settingService;
		private readonly IUrlHelperFactory _urlHelperFactory;

		#endregion

		#region Ctor

		public ProductQuantityDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
			IDiscountService discountService,
			ISettingService settingService,
			IUrlHelperFactory urlHelperFactory)
		{
			this._actionContextAccessor = actionContextAccessor;
			this._discountService = discountService;
			this._settingService = settingService;
			this._urlHelperFactory = urlHelperFactory;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Check discount requirement
		/// </summary>
		/// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
		/// <returns>Result</returns>
		public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			//invalid by default
			var result = new DiscountRequirementValidationResult();

			if (request.Customer == null)
				return result;


			var fromQuantity = _settingService.GetSettingByKey<int>(string.Format(DiscountRequirementDefaults.SettingsFromKey, request.DiscountRequirementId));
			var toQuantity = _settingService.GetSettingByKey<int>(string.Format(DiscountRequirementDefaults.SettingsToKey, request.DiscountRequirementId));
			if (fromQuantity == 0 && toQuantity == 0)
				return result;

			var cartQuery = from sci in request.Customer.ShoppingCartItems.LimitPerStore(request.Store.Id)
							where sci.ShoppingCartType == ShoppingCartType.ShoppingCart
							group sci by sci.ProductId into g
							select new { ProductId = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };

			var cart = cartQuery.ToList();
			var totalQuantity = cart.Sum(x => x.TotalQuantity);
			if (totalQuantity >= fromQuantity && totalQuantity <= toQuantity)
				result.IsValid = true;

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
			var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
			return urlHelper.Action("Configure", "DiscountRulesProductQuantity",
				new { discountId = discountId, discountRequirementId = discountRequirementId }).TrimStart('/');
		}

		/// <summary>
		/// Install the plugin
		/// </summary>
		public override void Install()
		{
			//locales
			this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.From", "From Quantity");
			this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.From.Hint", "Discount will be applied if cart has minimum this quantity.");
			this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.To", "To Quantity");
			this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.To.Hint", "Discount will be applied if cart has maimum this quantity.");

			base.Install();
		}

		/// <summary>
		/// Uninstall the plugin
		/// </summary>
		public override void Uninstall()
		{
			//discount requirements
			var discountRequirements = _discountService.GetAllDiscountRequirements()
				.Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SystemName);
			foreach (var discountRequirement in discountRequirements)
			{
				_discountService.DeleteDiscountRequirement(discountRequirement);
			}

			//locales           
			this.DeletePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.From");
			this.DeletePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.From.Hint");
			this.DeletePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.To");
			this.DeletePluginLocaleResource("Plugins.DiscountRules.ProductQuantity.Fields.To.Hint");

			base.Uninstall();
		}

		#endregion
	}
}