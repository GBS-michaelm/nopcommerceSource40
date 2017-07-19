using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.GBS
{
    public class GBSPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public bool UseSavedCards { get; set; }
        public TransactMode TransactMode { get; set; }
        //public string TransactionKey { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string GBSPaymentWebServiceAddress { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        //public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Additional fee
        /// </summary>
        //public decimal AdditionalFee { get; set; }
    }
}
