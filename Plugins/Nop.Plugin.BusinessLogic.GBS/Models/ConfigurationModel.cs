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
    }
}