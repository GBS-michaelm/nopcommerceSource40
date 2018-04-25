using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Models.Clients
{    
    public partial class ClientListModel : BaseNopModel
    {
        public ClientListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("MarketCenters.Client.CompanyName")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [NopResourceDisplayName("MarketCenters.Client.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }   
    }
}