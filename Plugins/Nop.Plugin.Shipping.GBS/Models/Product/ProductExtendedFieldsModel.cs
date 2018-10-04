using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System.ComponentModel;

namespace Nop.Plugin.Shipping.GBS.Models.Product
{
    public class ProductExtendedFieldsModel : BaseNopEntityModel
    {

        #region Admin Extended Filed 

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.ShippingCategoryA")]
        public string ShippingCategoryA { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.ShippingCategoryB")]

        public string ShippingCategoryB { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.Table.Name")]

        public string TableName { get; set; }

        #endregion

        #region Locale String Resor
     
        [DisplayName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Error")]
        public string ShippingOptionDesError { get; set; }

        [DisplayName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Success")]
        public string ShippingOptionDesSuccess { get; set; }

        [DisplayName("Plugins.Shipping.GBS.Product.ShippingOption.Description.Name")]
        public string ShippingOptionDesName { get; set; }

        #endregion
        
        #region Login/Password/Store Name 

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.LoginId")]
        public string LoginId { get; set; }
        public bool LoginId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.GBSShippingWebServiceAddress")]
        public string GBSShippingWebServiceAddress { get; set; }
        public bool GBSShippingWebServiceAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.GBSStoreNamePrepend")]
        public string GBSStoreNamePrepend { get; set; }
        public bool GBSStoreNamePrepend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.UseFlatRate")]
        public bool UseFlatRate { get; set; }
        public bool UseFlatRate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.GBS.Product.FlatRateAmount")]
        public decimal FlatRateAmount { get; set; }
        public bool FlatRateAmount_OverrideForStore { get; set; }

        #endregion
    }
}