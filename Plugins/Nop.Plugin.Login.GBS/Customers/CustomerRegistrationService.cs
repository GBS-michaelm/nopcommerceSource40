using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Services.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.Login.GBS;
using Nop.Services.Stores;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System;

namespace Nop.Services.Custom.Customers
{
    public class GBSCustomerRegistrationService : CustomerRegistrationService
    {
        private readonly GBSLoginSettings _gbsLoginSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger _logger;


        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerService">Customer service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="storeService">Store service</param>
        /// <param name="rewardPointService">Reward points service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="customerSettings">Customer settings</param>
        public GBSCustomerRegistrationService(
            IStoreContext storeContext,
            IPluginFinder pluginFinder,
            GBSLoginSettings gbsLoginSettings,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ILocalizationService localizationService,
            IStoreService storeService,
            IRewardPointService rewardPointService,
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings,
            ILogger logger)
            : base(
            customerService,
            encryptionService,
            newsLetterSubscriptionService,
            localizationService,
            storeService,
            rewardPointService,
            workContext,
            genericAttributeService,
            workflowMessageService,
            eventPublisher,
            rewardPointsSettings,
            customerSettings
             )
        {
            this._gbsLoginSettings = gbsLoginSettings;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._encryptionService = encryptionService;
            this._logger = logger;
        }


        #endregion
        public class LoginModel
        {
            public string username { get; set; }
            public string password { get; set; }
            public string pin { get; set; }
        }

        public class UserModel
        {
            public string emailAddress { get; set; }
            public string password { get; set; }
            public string currentPassword { get; set; }
            public string currentEmailAddress { get; set; }
            public bool updateEmail { get; set; }
            public bool updatePassword { get; set; }
        }


        public CustomerLoginResults ValidateHOM(string usernameOrEmail, string password)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            LoginModel login = new LoginModel();
            login.username = usernameOrEmail;
            login.password = password;
            login.pin = "111111";
            string loginJSON = JsonConvert.SerializeObject(login, Formatting.Indented);
            string responseString = client.UploadString(_gbsLoginSettings.GBSLoginWebServiceAddress, loginJSON);
            if (responseString.Contains("SUCCESS"))
            {
                //change password and return
                ChangePasswordRequest pRequest = new ChangePasswordRequest(usernameOrEmail, false, _customerSettings.DefaultPasswordFormat, password);
                ChangePasswordResult pResult = base.ChangePassword(pRequest);
                if (pResult.Errors.Count == 0)
                {
                    return CustomerLoginResults.Successful;
                }
                else
                {
                    string errors = string.Empty;

                    foreach (string item in pResult.Errors)
                    {
                        errors += " " + item.ToString();
                    }
                    throw new Exception("error changing password" + errors);
                }
            }
            else
            {
                return CustomerLoginResults.WrongPassword;
            }
        }

        public override CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {

            var miscPlugins = _pluginFinder.GetPlugins<MyLoginServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                CustomerLoginResults result = base.ValidateCustomer(usernameOrEmail, password);
                if (result != CustomerLoginResults.WrongPassword)
                {
                    return result;
                }
                else
                {
                    ValidateHOM(usernameOrEmail, password);
                }

            }
            return base.ValidateCustomer(usernameOrEmail, password);


        }

        public override CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {

            string errCode;
            CustomerRegistrationResult result = null;
            var results = new CustomerRegistrationResult();
            if (IsValidPassword(request.Password, out errCode))
            {
                result = base.RegisterCustomer(request);
            }
            else
            {
                switch (errCode.ToUpper())
                {
                    case "ERROR_PASSWORD_NO_LETTER":
                    case "ERROR_PASSWORD_NO_NUMBER":
                    case "ERROR_PASSWORD_LENGTH":
                        results.AddError("Passwords must contain at least 1 letter, 1 number, and be at least 8 characters long.");
                        break;
                    default:
                        break;
                }
                return results;
            }

            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<MyLoginServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {

                    

                    if (result.Errors.Count < 1)
                    {

                        WebClient client = new WebClient();
                        client.Headers.Add("Content-Type", "application/json");
                        client.Credentials = new NetworkCredential(_gbsLoginSettings.GBSCustomerWebServiceUserName, _gbsLoginSettings.GBSCustomerWebServicePassword);

                        UserModel model = new UserModel();
                        model.currentEmailAddress = request.Email;
                        model.emailAddress = request.Email;
                        model.currentPassword = request.Password;
                        model.password = request.Password;
                        model.updateEmail = false;
                        model.updatePassword = false;

                        string loginJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
                        string responseString = client.UploadString(_gbsLoginSettings.GBSRegisterWebService, loginJSON);
                        if (responseString.ToUpper() != "SUCCESS")
                        {
                            _logger.Error("RegisterCustomer() Override", new Exception(responseString), null);
                        }
                        switch (responseString.ToUpper())
                        {
                            case "ERROR_PASSWORD_NO_LETTER":
                            case "ERROR_PASSWORD_NO_NUMBER":
                            case "ERROR_PASSWORD_LENGTH":
                                result.AddError("Passwords must contain at least 1 letter, 1 number, and be at least 8 characters long.");
                                break;
                            default:
                                break;
                        }
                        return result;

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("RegisterCustomer() Override", ex, null);
            }

            return result;
           
        }

        public override ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            string errCode;
            ChangePasswordResult result = null;
            var results = new ChangePasswordResult();
            if (IsValidPassword(request.NewPassword, out errCode))
            {
                result = base.ChangePassword(request);
            }
            else
            {
                switch (errCode.ToUpper())
                {
                    case "ERROR_PASSWORD_NO_LETTER":
                    case "ERROR_PASSWORD_NO_NUMBER":
                    case "ERROR_PASSWORD_LENGTH":
                        results.AddError("Passwords must contain at least 1 letter, 1 number, and be at least 8 characters long.");
                        break;

                    default:
                        break;
                }
                return results;

            }
            
            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<MyLoginServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {


                    if (result.Errors.Count < 1)
                    {
                        WebClient client = new WebClient();
                        client.Headers.Add("Content-Type", "application/json");
                        client.Credentials = new NetworkCredential(_gbsLoginSettings.GBSCustomerWebServiceUserName, _gbsLoginSettings.GBSCustomerWebServicePassword);
                        UserModel model = new UserModel();
                        model.currentEmailAddress = request.Email;
                        model.emailAddress = request.Email;
                        model.currentPassword = request.OldPassword;
                        model.password = request.NewPassword;
                        model.updateEmail = false;
                        model.updatePassword = true;

                        string loginJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
                        string responseString = client.UploadString(_gbsLoginSettings.GBSUpdateCustomerWebService, loginJSON);

                        if (responseString.ToUpper() != "SUCCESS")
                        {
                            _logger.Error("ChangePassword() Override", new Exception(responseString), null);
                        }
                        switch (responseString.ToUpper())
                        {
                            case "ERROR_PASSWORD_NO_LETTER":
                            case "ERROR_PASSWORD_NO_NUMBER":
                            case "ERROR_PASSWORD_LENGTH":
                                result.AddError("Passwords must contain at least 1 letter, 1 number, and be at least 8 characters long.");
                                break;
                             
                            default:
                                break;
                        }

                            return result;
                        }
               
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ChangePassword() Override", ex, null);
            }
            return result;
        }
        public override void SetUsername(Customer customer, string newUsername)
        {
            base.SetUsername(customer, newUsername);
            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<MyLoginServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {
                    string emailOld = customer.Email;
                   
              
                        WebClient client = new WebClient();
                        client.Headers.Add("Content-Type", "application/json");
                    client.Credentials = new NetworkCredential(_gbsLoginSettings.GBSCustomerWebServiceUserName, _gbsLoginSettings.GBSCustomerWebServicePassword);
                    UserModel model = new UserModel();
                        model.currentEmailAddress = emailOld;
                        model.emailAddress = newUsername;
                        //model.currentPassword = customer.Password;
                        //model.password = customer.Password;
                        model.updateEmail = true;
                        model.updatePassword = false;

                        string loginJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
                        string responseString = client.UploadString(_gbsLoginSettings.GBSUpdateCustomerWebService , loginJSON);
                
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SetUsername() Override", ex, null);
            }
        }
        public override void SetEmail(Customer customer, string newEmail, bool requireValidation)
        {
            base.SetEmail(customer, newEmail, requireValidation);
            try { 
                var miscPlugins = _pluginFinder.GetPlugins<MyLoginServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {
                    string emailOld = customer.Email;
                   
               
                        WebClient client = new WebClient();
                        client.Headers.Add("Content-Type", "application/json");
                        client.Credentials = new NetworkCredential(_gbsLoginSettings.GBSCustomerWebServiceUserName, _gbsLoginSettings.GBSCustomerWebServicePassword);
                        UserModel model = new UserModel();
                        model.currentEmailAddress = emailOld;
                        model.emailAddress = newEmail;
                        //model.currentPassword = customer.Password;
                        //model.password = customer.Password;
                        model.updateEmail = true;
                        model.updatePassword = false;

                        string loginJSON = JsonConvert.SerializeObject(model, Formatting.Indented);
                        string responseString = client.UploadString(_gbsLoginSettings.GBSUpdateCustomerWebService, loginJSON);
               
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SetEmail() Override", ex, null);
            }

        }

        public bool IsValidPassword(string password, out string errCode)
        {
            if (string.IsNullOrEmpty(password))
            {
                errCode = "ERROR_EMPTY_PASSWORD";
                return false;
            }

            if (password.Length < 8)
            {
                errCode = "ERROR_PASSWORD_LENGTH";
                return false;
            }

            bool hasNumber = false;
            bool hasLetter = false;

            foreach (char pwChar in password)
            {
                int asciiVal = (int)pwChar;

                if ((asciiVal >= 65 && asciiVal <= 90) ||
                   (asciiVal >= 97 && asciiVal <= 122))
                {
                    hasLetter = true;
                }
                else if (asciiVal >= 48 && asciiVal <= 57)
                {
                    hasNumber = true;
                }
            }

            if (!hasNumber)
            {
                errCode = "ERROR_PASSWORD_NO_NUMBER";
                return false;
            }

            if (!hasLetter)
            {
                errCode = "ERROR_PASSWORD_NO_LETTER";
                return false;
            }

            errCode = "";
            return true;
        }


    }
}
