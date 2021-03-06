﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;

namespace Nop.Plugin.Catalog.GBS.Controllers
{
    public class GBSCatalogController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogModelFactory _catalogModelFactory;

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public GBSCatalogController(
            ICategoryService categoryService,
            ICatalogModelFactory catalogModelFactory,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            this._categoryService = categoryService;
            this._catalogModelFactory = catalogModelFactory;

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public IActionResult SportsTeamsList(IList<Category> model, int sportsTeamId)
        {
            ViewBag.Id = sportsTeamId;
            return View(model);
        }

        public ActionResult SportsTeamsTest()
        {
            ViewBag.Id = 1;
            return View();
        }
    }
}