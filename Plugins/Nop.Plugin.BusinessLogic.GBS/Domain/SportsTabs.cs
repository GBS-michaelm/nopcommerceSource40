using System;
using System.Linq;
using System.Collections.Generic;
using Nop.Plugin.BusinessDataAccess.GBS;
using System.Data;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Core.Caching;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using Newtonsoft.Json;
using Nop.Plugin.BusinessLogic.GBS.Factories;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    public class Tab
    {
        public string tabName { get; set; }
        public int id { get; set; }
        public List<TabData> TeamLinks { get; set; } = new List<TabData>();
    }

    public class TabData
    {
        public string teamName { get; set; }
        public string teamLink { get; set; }
    }
    
    public class SportsTabs
    {
        ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        ILogger logger = EngineContext.Current.Resolve<ILogger>();

        public string H1 { get; set; }
        public string H2 { get; set; }
        public string UpperText { get; set; }
        public string LowerText { get; set; }
        public string BackgroundPicturePath { get; set; }
        public string ForegroundPicturePath { get; set; }
        public string BackgroundColor { get; set; }
        public List<Tab> tabs = new List<Tab>();

        public static SportsTabs GetSportsTabs(int sportCategoryId)
        {
            DBManager SPORTSTABMANAGER = new DBManager();

            Dictionary<string, Object> sportsTeamTabDic = new Dictionary<string, Object>();
            string select = "EXEC usp_SELECTGBSGetSportsTeam @categoryId";
            sportsTeamTabDic.Add("@categoryId", sportCategoryId);

            string jsonResult = SPORTSTABMANAGER.GetParameterizedJsonString(select, sportsTeamTabDic);

            List<SportsTabs> sportsTabsList = JsonConvert.DeserializeObject<List<SportsTabs>>(jsonResult);

            return sportsTabsList[0];

        }
        
    }
    
}
