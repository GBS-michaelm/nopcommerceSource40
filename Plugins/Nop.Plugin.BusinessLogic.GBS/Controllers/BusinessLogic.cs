using Nop.Web.Framework.Controllers;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.BusinessLogic.GBS.Controllers
{

    [AdminAuthorize]

    public class BusinessLogic : BasePluginController
    {

        public ActionResult Configure()
        {
            return View("~/Plugins/BusinessLogic.GBS/Views/BusinessLogic/Configure.cshtml");
        }

    }

    

}
