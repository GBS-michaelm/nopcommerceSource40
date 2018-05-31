using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.BusinessLogic.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }  

        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.Hack")]
        public bool Hack { get; set; }
        public bool Hack_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.MarketCenterDefaultId")]
        public int MarketCenterDefaultId { get; set; }
        public bool MarketCenterDefaultId_OverrideForStore { get; set; }

        
        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.MarketCenterNameBadgeDefaultId")]
        public int MarketCenterNameBadgeDefaultId { get; set; }
        public bool MarketCenterNameBadgeDefaultId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.MarketCenterBusinessCardDefaultId")]
        public int MarketCenterBusinessCardDefaultId { get; set; }
        public bool MarketCenterBusinessCardDefaultId_OverrideForStore { get; set; }

        
        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.MarketCenterCarMagnetDefaultId")]
        public int MarketCenterCarMagnetDefaultId { get; set; }
        public bool MarketCenterCarMagnetDefaultId_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.BusinessLogic.GBS.Fields.MarketCenterWhatAmIReferenceName")]
        public string MarketCenterWhatAmIReferenceName { get; set; }
        public bool MarketCenterWhatAmIReferenceName_OverrideForStore { get; set; }

    }
}