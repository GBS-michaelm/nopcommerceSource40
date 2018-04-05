using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.BusinessLogic.GBS.Domain;

namespace Nop.Plugin.BusinessLogic.GBS.Models
{
    public class MarketCenterGatewayTabsModel
    {
        public List<MarketCenterGatewayTabModel> MarketCenterTabsList = new List<MarketCenterGatewayTabModel>();
        public string hiddenHtml;
        public string hiddenAll;
    }
}
