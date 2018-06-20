using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using System;

namespace Nop.Plugin.Logging.GBS.Logging
{
    class GBSLogger : DefaultLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GBSLogger(IRepository<Log> logRepository, IWebHelper webHelper, Nop.Data.IDbContext dbContext, IDataProvider dataProvider, CommonSettings commonSettings,
            IHttpContextAccessor httpContextAccessor) : 
            base(logRepository, webHelper, dbContext, dataProvider, commonSettings)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
        {
            try
            {
                //HttpBrowserCapabilities bc = _httpContextAccessor.HttpContext.Request.Headers[""];
                //fullMessage += "Browser attributes - User Agent: " + bc.Browser + "; Version: " + bc.Version + "; Type: " + bc.Type + "; IsMobileDevice: " + bc.IsMobileDevice + "; DeviceManufacturer: " + bc.MobileDeviceManufacturer + "; DeviceModel: " + bc.MobileDeviceModel + "; ScreenWidth: " + bc.ScreenPixelsWidth + "; ScreenHeight: " + bc.ScreenPixelsHeight;
            }catch (Exception exx)
            {

            }
            return base.InsertLog(logLevel, shortMessage, fullMessage, customer);

        }

    }
}
