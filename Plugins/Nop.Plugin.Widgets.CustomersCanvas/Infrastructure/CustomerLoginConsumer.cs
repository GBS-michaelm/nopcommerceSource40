using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class CustomerLoginConsumer : IConsumer<CustomerLoggedinEvent>
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IRepository<CcDesign> _ccDesignRepository;
        public CustomerLoginConsumer(IWorkContext workContext,
            ISettingService settingService,
            ILogger logger,
            IRepository<CcDesign> ccDesignRepository)
        {
            _workContext = workContext;
            _settingService = settingService;
            _logger = logger;
            _ccDesignRepository = ccDesignRepository;
        }
        public void HandleEvent(CustomerLoggedinEvent eventMessage)
        {
            try
            {
                var oldUserId = _workContext.CurrentCustomer.Id;
                var newUserId = eventMessage.Customer.Id;
                var userNamePrefix = "nopcommerce_";
                
                var oldCCUserGUID = userNamePrefix + oldUserId;
                var newCCUserGUID = userNamePrefix + newUserId;

                var designs = _ccDesignRepository.Table.Where(x => 
                    ((x.ImageUrl != null && x.ImageUrl.IndexOf(oldCCUserGUID) > -1) || 
                    (x.DownloadUrlsJson != null && x.DownloadUrlsJson.IndexOf(oldCCUserGUID) > -1)))
                    .ToList();

                foreach(var ccDesign in designs)
                {
                    ccDesign.ImageUrl = ccDesign.ImageUrl.Replace(oldCCUserGUID, newCCUserGUID);
                    ccDesign.DownloadUrlsJson = ccDesign.DownloadUrlsJson.Replace(oldCCUserGUID, newCCUserGUID);
                    _ccDesignRepository.Update(ccDesign);
                }

                var settings = _settingService.LoadSetting<CcSettings>();
                var baseUrl = settings.ServerHostUrl;
                var url = baseUrl + "/api/User/MergeUsers";

                var data = new MergeUsersData();
                data.source = userNamePrefix + oldUserId;
                data.destination = userNamePrefix + newUserId;

                var client = new HttpClient();
                var response = client.PostAsJsonAsync(url, data).Result;
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.Error("Error in userLoginEvent. in MergeUsers CC action", ex);
            }

        }
    }

    public class MergeUsersData
    {
        public string source;
        public string destination;
    }

}
