using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.BusinessLogic.GBS.Models
{
    public class GBSOutputCacheAttribute : OutputCacheAttribute
    {
        public GBSOutputCacheAttribute()
        {
            ISettingService settingService = EngineContext.Current.Resolve<ISettingService>();
            IStoreContext storeContext = EngineContext.Current.Resolve<IStoreContext>();
            this.Duration = 3600;
            int duration = 0;
            var success = Int32.TryParse(settingService.LoadSetting<GBSBusinessLogicSettings>(storeContext.CurrentStore.Id).CacheDurationInSeconds, out duration);
            if (!success)
            {
                this.Duration = 3600;
            }else
            {
                this.Duration = duration;
            }

        }
    }
}
