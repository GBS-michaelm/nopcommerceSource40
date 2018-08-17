using System.Collections.Generic;

namespace Nop.Plugin.Payments.GBS.Models
{
    //[Table("tblCustomers_BillingAddress")]
    public class PaymentMethodModel
    {
        private string _cardType;
        public int profileID { get; set; }
        public string Last4Digits { get; set; }
        public string CardType {
            get {

                switch (_cardType)
                {
                    case "VI":
                        return "visa";
                        break;
                    case "MC":
                        return "mastercard";
                        break;
                    case "AX":
                        return "amex";
                        break;
                    case "DI":
                        return "discover";
                        break;
                    default:
                        return "visa";
                        break;
                }
            }
            set { _cardType = value; }
        }
        public string UCardType
        {
            get
            {

                switch (_cardType)
                {
                    case "VI":
                        return "Visa";
                        break;
                    case "MC":
                        return "Mastercard";
                        break;
                    case "AX":
                        return "Amex";
                        break;
                    case "DI":
                        return "Discover";
                        break;
                    default:
                        return "Visa";
                        break;
                }
            }
        }
        public string NickName { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
    }

    public class CustomerPaymentProfilesModel
    {
        public int customerID { get; set; }
        public IList<PaymentMethodModel> SavedProfiles { get; set; }

        public CustomerPaymentProfilesModel()
        {
            SavedProfiles = new List<PaymentMethodModel>();
        }

    }
}
