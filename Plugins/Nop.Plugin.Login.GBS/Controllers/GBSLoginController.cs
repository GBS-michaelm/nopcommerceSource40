using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Security.Honeypot;
using Nop.Web.Models.Customer;
using Nop.Web.Controllers;
using Nop.Plugin.Login.GBS.Models;
using Nop.Core.Infrastructure;
using Nop.Services.Custom.Customers;
namespace Nop.Plugin.Login.GBS.Controllers
{
    public partial class GBSLoginController : BasePublicController
    {
        private Nop.Web.Controllers.CustomerController _baseNopCustomerController;

        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ITaxService _taxService;
        private readonly CustomerSettings _customerSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IStoreService _storeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public GBSLoginController(IAddressModelFactory addressModelFactory,
            ICustomerModelFactory customerModelFactory,
            IAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            ITaxService taxService,
            CustomerSettings customerSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            IAddressService addressService,
            ICountryService countryService,
            IOrderService orderService,
            IPictureService pictureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IShoppingCartService shoppingCartService,
            IOpenAuthenticationService openAuthenticationService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IStoreService storeService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            StoreInformationSettings storeInformationSettings,
            ILogger logger, 
            CaptchaSettings _captchaSettings) 
        {
            this._addressModelFactory = addressModelFactory;
            this._customerModelFactory = customerModelFactory;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerService = customerService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._taxService = taxService;
            this._customerSettings = customerSettings;
            this._addressSettings = addressSettings;
            this._forumSettings = forumSettings;
            this._addressService = addressService;
            this._countryService = countryService;
            this._orderService = orderService;
            this._pictureService = pictureService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._openAuthenticationService = openAuthenticationService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._storeService = storeService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._logger = logger;

            this._baseNopCustomerController = new Nop.Web.Controllers.CustomerController(
            this._addressModelFactory,
            this._customerModelFactory,
            this._authenticationService,
            this._dateTimeSettings,
            this._taxSettings,
            this._localizationService,
            this._workContext,
            this._storeContext,
            this._customerService,
            this._customerAttributeParser,
            this._customerAttributeService,
            this._genericAttributeService,
            this._customerRegistrationService,
            this._taxService,
            this._customerSettings,
            this._addressSettings,
            this._forumSettings,
            this._addressService,
            this._countryService,
            this._orderService,
            this._pictureService,
            this._newsLetterSubscriptionService,
            this._shoppingCartService,
            this._openAuthenticationService,
            this._webHelper,
            this._customerActivityService,
            this._addressAttributeParser,
            this._addressAttributeService,
            this._storeService,
            this._eventPublisher,
            this._mediaSettings,
            this._workflowMessageService,
            this._localizationSettings,
            this._captchaSettings,
            this._storeInformationSettings);
            
        }
        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoginModal(LoginModel model)
        {
            try
            {
                ResponseModel response = new ResponseModel();
                var customerValidation = _customerRegistrationService.ValidateCustomer(model.Email, model.Password);

                switch (customerValidation)
                {
                    case CustomerLoginResults.Successful:
                        var baseAR = _baseNopCustomerController.Login(model, "", true);
                        response.message = "Success";
                        return Json(response);
                    case CustomerLoginResults.WrongPassword:
                        response.message = "Wrong Password";
                        return Json(response);
                    case CustomerLoginResults.CustomerNotExist:
                        response.message = "Customer Not Exist";
                        return Json(response);
                    case CustomerLoginResults.NotRegistered:
                        response.message = "Customer Not Exist";
                        return Json(response);
                    default:
                        break;
                }

                response.message = "Failed";
                return Json(response);


        }
            catch(Exception ex)
            {
                _logger.Error("Error in LoginModal", ex);
                ResponseModel response = new ResponseModel();
                response.status = "failure";
                response.message = ex.Message;
                return Json(response);
            }
        }
    }
}
