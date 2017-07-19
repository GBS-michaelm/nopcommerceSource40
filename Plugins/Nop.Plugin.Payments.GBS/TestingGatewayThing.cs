using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebServices.Models.Payment;

namespace Testing.GBSPaymentGateway
{



    public class GBSPaymentGateway
    {
        //GBSPaymentGateway() {}


        public GBSTransactionResponse AuthorizeAndCapture(PaymentTransactionModel payment, string address)
        {

            GBSTransactionResponse authTransaction = new GBSTransactionResponse();

            try
            {

                WebClient client = new WebClient();
                client.Headers.Add("Content-Type", "application/json");
                string responseString = client.UploadString(address, payment.ToJSON());

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(responseString);

                XmlNode procStatusNode = xml.SelectSingleNode("//ProcStatus");
                XmlNode approvalStatusNode = xml.SelectSingleNode("//ApprovalStatus");
                XmlNode statusMessageNode = xml.SelectSingleNode("//StatusMsg");
                XmlNode responseMessageNode = xml.SelectSingleNode("//RespMsg");
                XmlNode transactionReferenceNumberNode = xml.SelectSingleNode("//TxRefNum");
                XmlNode authorizationCodeNode = xml.SelectSingleNode("//AuthCode");

                authTransaction.transactId = transactionReferenceNumberNode.InnerXml;
                authTransaction.authCode = authorizationCodeNode.InnerXml; //String.IsNullOrEmpty(authorizationCodeNode.InnerXml) ? "000000" : 




                switch (procStatusNode.InnerXml)
                {
                    case "0":
                        authTransaction.resultCode = GBSTransactionResponse.ResultCodeType.Success;
                        switch (approvalStatusNode.InnerXml)
                        {
                            case "0": //Declined
                                authTransaction.responseCode = GBSTransactionResponse.ResponseCodeType.Declined;
                                authTransaction.messages.Add(statusMessageNode.InnerXml);
                                authTransaction.messages.Add(responseMessageNode.InnerXml);
                                break;
                            case "1": //Approved
                                authTransaction.responseCode = GBSTransactionResponse.ResponseCodeType.Approved;
                                authTransaction.messages.Add(statusMessageNode.InnerXml);
                                authTransaction.messages.Add(responseMessageNode.InnerXml);
                                break;
                            default: //Error
                                authTransaction.responseCode = GBSTransactionResponse.ResponseCodeType.Error;
                                authTransaction.errors.Add(statusMessageNode.InnerXml);
                                authTransaction.errors.Add(responseMessageNode.InnerXml);
                                break;
                        }
                        break;

                    default:
                        authTransaction.resultCode = GBSTransactionResponse.ResultCodeType.Failure;
                        authTransaction.responseCode = GBSTransactionResponse.ResponseCodeType.Error;
                        authTransaction.errors.Add(statusMessageNode.InnerXml);
                        authTransaction.errors.Add(responseMessageNode.InnerXml);
                        break;
                }

                switch (authTransaction.resultCode)
                {
                    case GBSTransactionResponse.ResultCodeType.Success:

                        switch (authTransaction.responseCode)
                        {
                            case GBSTransactionResponse.ResponseCodeType.Approved:
                                //do nothing
                                break;
                            case GBSTransactionResponse.ResponseCodeType.Declined:
                                break;
                            case GBSTransactionResponse.ResponseCodeType.Error:
                                break;
                            default:
                                break;
                        }
                        break;
                    case GBSTransactionResponse.ResultCodeType.Failure:
                        break;
                    default:
                        break;
                }



                return authTransaction;
            }
            catch (Exception ex)
            {
                //authOutput.Value = ex.Message + " ERROR!! ";
                return new GBSTransactionResponse();
            }

        }
    }

    public class GBSTransactionResponse
    {
        public GBSTransactionResponse() { }

        private ResultCodeType _resultCode;
        private ResponseCodeType _responseCode;
        private List<string> _messages = new List<string>();
        private List<string> _errors = new List<string>();
        public enum ResultCodeType { Success, Failure }
        public enum ResponseCodeType { Declined, Approved,  Error }
        private string _transactId;
        private string _authCode;
        public ResultCodeType resultCode
        {
            get
            {
                return _resultCode;
            }
            set
            {
                _resultCode = value;
            }
        }
        public ResponseCodeType responseCode
        {
            get
            {
                return _responseCode;
            }
            set
            {
                _responseCode = value;
            }
        }
        public List<string> messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }
        public List<string> errors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
            }
        }
        public string transactId
        {
            get
            {
                return _transactId;
            }
            set
            {
                _transactId = value;
            }
        }
        public string authCode
        {
            get
            {
                return _authCode;
            }
            set
            {
                _authCode = value;
            }
        }
    }
}
