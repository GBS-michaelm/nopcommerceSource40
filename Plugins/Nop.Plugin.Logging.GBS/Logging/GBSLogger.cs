using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using Nop.Core.Domain.Customers;
using System.Web;

namespace Nop.Plugin.Logging.GBS.Logging
{
    class GBSLogger : DefaultLogger
    {
        public GBSLogger(IRepository<Log> logRepository, IWebHelper webHelper, Nop.Data.IDbContext dbContext, IDataProvider dataProvider, CommonSettings commonSettings) : 
            base(logRepository, webHelper, dbContext, dataProvider, commonSettings)
        {
        }
        public override Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            try
            {
                HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                fullMessage += "Browser attributes - User Agent: " + bc.Browser + "; Version: " + bc.Version + "; Type: " + bc.Type + "; IsMobileDevice: " + bc.IsMobileDevice + "; DeviceManufacturer: " + bc.MobileDeviceManufacturer + "; DeviceModel: " + bc.MobileDeviceModel + "; ScreenWidth: " + bc.ScreenPixelsWidth + "; ScreenHeight: " + bc.ScreenPixelsHeight;
            }catch (Exception exx)
            {

            }
            return base.InsertLog(logLevel, shortMessage, fullMessage, customer);

        }

    }
}
