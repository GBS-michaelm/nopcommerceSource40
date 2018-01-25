using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Plugin.Catalog.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Catalog.GBS.AllCategory")]
        public bool AllCategory { get; set; }
        public bool AllCategory_OverrideForStore { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Catalog.GBS.NoOfChildren")]
        public int NoOfChildren { get; set; }
        public bool NoOfChildren_OverrideForStore { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Catalog.GBS.IsActive")]
        public bool IsActive { get; set; }
        public bool IsActive_OverrideForStore { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Catalog.GBS.Blacklist")]
        public string BlackList { get; set; }
        public bool BlackList_OverrideForStore { get; set; }

    }
}