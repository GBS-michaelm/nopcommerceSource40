
namespace Nop.Plugin.DiscountRules.ProductQuantity
{
    /// <summary>
    /// Represents constants for the discount requirement rule
    /// </summary>
    public static class DiscountRequirementDefaults
    {
        /// <summary>
        /// The system name of the discount requirement rule
        /// </summary>
        public const string SystemName = "DiscountRequirement.MustBeAssignedAsPerProductQuantity";
		
		/// <summary>
		/// The settings "From" key
		/// </summary>
		public const string SettingsFromKey = "DiscountRequirement.MustBeAssignedAsPerProductQuantity-From-{0}";

		/// <summary>
		/// The settings "To" key
		/// </summary>
		public const string SettingsToKey = "DiscountRequirement.MustBeAssignedAsPerProductQuantity-To-{0}";

		/// <summary>
		/// The HTML field prefix for discount requirements
		/// </summary>
		public const string HtmlFieldPrefix = "DiscountRulesProductQuantity{0}";
    }
}
