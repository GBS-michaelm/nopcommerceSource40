using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Domain
{
    public class MC_Client : BaseEntity
    {
        public string Name { get; set; }
        
        public string Parent { get; set; }

        public string AccountRep { get; set; }
        
        public int Proofs { get; set; }

        public bool InMenu { get; set; }
    }
}
