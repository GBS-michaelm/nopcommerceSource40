using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.DiscountRules.HasAttribute.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.HasAttribute.Fields.Attributes")]
        public string Attributes { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        #region Nested classes

        public partial class AddAttributeModel : BaseNopModel
        {
            public AddAttributeModel()
            {
                AvailableAttributes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Attribute")]
            public int SearchAttributeId { get; set; }


            public IList<SelectListItem> AvailableAttributes { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

        public partial class AttributeModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool Published { get; set; }
        }
        #endregion
    }
}