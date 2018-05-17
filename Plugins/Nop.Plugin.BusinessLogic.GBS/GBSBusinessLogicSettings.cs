using Nop.Core.Configuration;

namespace Nop.Plugin.BusinessLogic.GBS
{
    public class GBSBusinessLogicSettings : ISettings
    {       
        public bool Hack { get; set; }
        public int MarketCenterDefaultId { get; set; }

        public string MarketCenterWhatAmIReferenceName { get; set; }

    }
}
