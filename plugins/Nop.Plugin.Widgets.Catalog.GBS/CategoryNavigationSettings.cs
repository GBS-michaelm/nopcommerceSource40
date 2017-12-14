using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Catalog.GBS
{
    public class CategoryNavigationSettings : ISettings
    {        
        public bool AllCategory { get; set; }
        public int NoOfChildren { get; set; }        
        public bool IsActive { get; set; }
    }
}
