using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Models.Clients
{
    public partial class ContactModel : BaseNopEntityModel
    {
        public ContactModel()
        {
            AvailableContactNames = new List<SelectListItem>();            
        }
        // Account Contact
        [NopResourceDisplayName("MarketCenters.Client.Contact.ContactName")]
        //[DisplayName("Contact Name")]                
        public string ContactName { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.Contact.ContactEmail")]
        //[DisplayName("Contact Email")]
        public string ContactEmail { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.Contact.ContactPhone")]
        //[DisplayName("Contact Phone")]
        public string ContactPhone { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.Contact.ContactNameGBS")]
        //[DisplayName("Contact Name")]
        public int ContactNameGBS { get; set; }
        public IList<SelectListItem> AvailableContactNames { get; set; }       
    }
}