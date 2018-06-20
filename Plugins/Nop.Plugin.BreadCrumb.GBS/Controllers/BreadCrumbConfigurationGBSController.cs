using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.BreadCrumb.GBS.Controllers
{
    [Area(AreaNames.Admin)]
    public class BreadCrumbGBSController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;


        public BreadCrumbGBSController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _storeService = storeService;
            _workContext = workContext;
        }
                
        public IActionResult Configure()
        {       
            return View();
        }
    }       
}