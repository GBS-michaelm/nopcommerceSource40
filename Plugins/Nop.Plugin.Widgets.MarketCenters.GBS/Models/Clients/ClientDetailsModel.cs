using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Models.Clients
{
    public partial class ClientDetailsModel : BaseNopEntityModel
    {
        public ClientDetailsModel()
        {
            AvailableParentCompany = new List<SelectListItem>();
            AvailableParentCompany = new List<SelectListItem>();
            AvailableRelatedCompaneis = new List<SelectListItem>();
            LeadingHeaderList = new List<SelectListItem>();            
        }
        // Company Name
        [NopResourceDisplayName("MarketCenters.Client.Id")]
        public override int Id { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.ShortVariableName")]
        public string ShortVariableName { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        //Relationships
        [NopResourceDisplayName("MarketCenters.Client.ParentCompany")]
        public int ParentCompany { get; set; }
        public IList<SelectListItem> AvailableParentCompany { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.RelatedCompany")]
        public int RelatedCompany { get; set; }
        public IList<SelectListItem> AvailableRelatedCompaneis { get; set; }

        // Branding
        [NopResourceDisplayName("MarketCenters.Client.Visible")]
        public bool Visible { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.LogoDisplay")]

        public bool LogoDisplay { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.AlternateLogoId")]
        [UIHint("Picture")]
        public int AlternateLogoId { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.LeadingHeader")]

        public int LeadingHeader { get; set; }

        public IList<SelectListItem> LeadingHeaderList { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.LeadingCustomHeader")]
        [AllowHtml]
        public string LeadingCustomHeader { get; set; }

        //Access
        [NopResourceDisplayName("MarketCenters.Client.UrlStub")]
        public string UrlStub { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AclCustomerRoles")]

        public string SelectedCustomerRoleIds { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.LimitedToStores")]
        public string SelectedStoreIds { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.AuthorizeAPI")]
        public bool AuthorizeAPI { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.RequirePassword")]
        public bool RequirePassword { get; set; }


        // Used for Client List

        [NopResourceDisplayName("MarketCenters.Client.Parent")]
        public string Parent { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.AccountRep")]
        public string AccountRep { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.Proofs")]
        public int Proofs { get; set; }
        [NopResourceDisplayName("MarketCenters.Client.InMenu")]
        public bool InMenu { get; set; }
    }
}