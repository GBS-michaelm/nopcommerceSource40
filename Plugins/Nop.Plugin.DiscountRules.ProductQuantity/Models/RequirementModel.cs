using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.ProductQuantity.Models
{
	public class RequirementModel
    {
        public RequirementModel()
        {
            
        }

        [NopResourceDisplayName("Plugins.DiscountRules.ProductQuantity.Fields.From")]
        public int FromQuantity { get; set; }

		[NopResourceDisplayName("Plugins.DiscountRules.ProductQuantity.Fields.To")]
		public int ToQuantity { get; set; }

		public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}