using Nop.Core.Configuration;

namespace Nop.Plugin.BusinessLogic.GBS
{
    public class GBSBusinessLogicSettings : ISettings
    {       
        public bool Hack { get; set; }
        public int MarketCenterDefaultId { get; set; }
        public int MarketCenterNameBadgeDefaultId { get; set; }
        public int MarketCenterBusinessCardDefaultId { get; set; }
        public int MarketCenterCarMagnetDefaultId { get; set; }
        public string MarketCenterWhatAmIReferenceName { get; set; }

    }
}
