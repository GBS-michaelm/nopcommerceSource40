using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GBS.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.UseSavedCards")]
        public bool UseSavedCards { get; set; }
        public bool UseSavedCards_OverrideForStore { get; set; }

        public int TransactModeId { get; set; }
        public bool TransactModeId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.TransactModeValues")]
        public SelectList TransactModeValues { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.GBS.Fields.TransactionKey")]
        //public string TransactionKey { get; set; }
        //public bool TransactionKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.LoginId")]
        public string LoginId { get; set; }
        public bool LoginId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GBS.Fields.GBSPaymentWebServiceAddress")]
        public string GBSPaymentWebServiceAddress { get; set; }
        public bool GBSPaymentWebServiceAddress_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.GBS.Fields.AdditionalFee")]
        //public decimal AdditionalFee { get; set; }
        //public bool AdditionalFee_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.GBS.Fields.AdditionalFeePercentage")]
        //public bool AdditionalFeePercentage { get; set; }
        //public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}