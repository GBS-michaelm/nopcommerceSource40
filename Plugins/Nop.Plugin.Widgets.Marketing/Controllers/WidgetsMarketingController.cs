using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Nop.Services;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Core.Domain.Localization;
using Nop.Core.Data;
using Nop.Core;
using Nop.Web.Models.Common;
using Nop.Core.Caching;
using Nop.Web.Infrastructure.Cache;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.Marketing.Controllers
{
    public class WidgetsMarketingController : BasePluginController
    {
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {

            return View("~/Plugins/Widgets.Marketing/Views/PublicInfo.cshtml");
        }

    }
}
