using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Nop.Web.Infrastructure.MarketCenter
{
    public class CdnManager
    {
        private Dictionary<string, CdnItem> cdnItems;
        private class CdnItem
        {
            public DateTime Expires { get; set; }
        }

        public CdnManager() {
            cdnItems = new Dictionary<string, CdnItem>();
        }
        
        public bool RemoveFromCache(string url) {
            if (!cdnItems.ContainsKey(url) || (cdnItems.ContainsKey(url) && cdnItems[url].Expires < DateTime.Now))
            {
                if (Remove(url))
                {
                    ISettingService _settingService = EngineContext.Current.Resolve<ISettingService>();
                    var currentStoreContext = EngineContext.Current.Resolve<IStoreContext>();
                    var cacheDuration = _settingService.GetSettingByKey<string>("cloudflaresettings.cacheduration", null, currentStoreContext.CurrentStore.Id, true);
                    var cacheDurationValue = string.IsNullOrEmpty(cacheDuration) ? 10.00 : double.Parse(cacheDuration);
                    cdnItems[url] = new CdnItem() { Expires = DateTime.Now.AddMinutes(cacheDurationValue) };
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool Remove(string url) {
            string responseString = null;
            List<string> urls = new List<string>();
            try
            {
                urls.Add(url);
                var urlsArray = urls.ToArray();


                ISettingService _settingService = EngineContext.Current.Resolve<ISettingService>();
                var currentStoreContext = EngineContext.Current.Resolve<IStoreContext>();
                var purgeEnabled = _settingService.GetSettingByKey<string>("cloudflaresettings.purge_enabled", null, currentStoreContext.CurrentStore.Id, true);
                bool purgeCDN = false;
                bool.TryParse(purgeEnabled, out purgeCDN);
                if (purgeCDN)
                {
                    WebClient client = new WebClient();
                    var address = "https://api.cloudflare.com/client/v4/zones/b1e7a58364845797357923f1da33ccff/purge_cache";
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("X-Auth-Email", "mattm@gogbs.com");
                    client.Headers.Add("X-Auth-Key", "8c1b2fdebc86bd1aefa7f8414e5f709ed6761");
                    dynamic data = new ExpandoObject();
                    data.files = urlsArray;
                    var dataJson = JsonConvert.SerializeObject(data);
                    responseString = client.UploadString(address, "DELETE", dataJson);

                }
                return true;
            }
            catch (Exception ex)
            {
                ILogger _logger = EngineContext.Current.Resolve<ILogger>();
                _logger.Error("Exception in DeleteCDNCache, urls =  " + string.Join(",", urls) + " | responseString = " + responseString, ex);
                return false;
            }
        }
    }

    public class Helpers
    {
        private static CdnManager cdnManager = new CdnManager();
        public static CdnManager CdnManager {
            get {
                if (cdnManager == null)
                {
                    cdnManager = new CdnManager();
                }
                return cdnManager;
            }
        }
        
        public static bool isCategoryMarketCenterProductCategory(int categoryId) {
            Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
            paramDicEx.Add("@id", categoryId);
            var isMarketCenterProduct = false;
            DBManager manager = new DBManager();
            var dv = manager.GetParameterizedDataView("EXEC usp_CheckIfMarketCenterProductByCategoryId  @id", paramDicEx);
            if (dv.Count > 0 && dv[0]["NumberOfRecords"] != DBNull.Value && Convert.ToInt32(dv[0]["NumberOfRecords"]) > 0) //This checks if category is tied to a market center
            {
                isMarketCenterProduct = true;
            }
            return isMarketCenterProduct;
        }

        public static bool isCategoryMarketCenterCompanyOrOfficetCategory(int categoryId) {
            Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
            paramDicEx.Add("@id", categoryId);
            var isMarketCenterCompany = false;
            DBManager manager = new DBManager();
            var dv = manager.GetParameterizedDataView("EXEC usp_SelectTblNopCategory  @id", paramDicEx);
            if (dv.Count > 0 && dv[0]["TypeId"] != DBNull.Value && Int32.Parse(dv[0]["TypeId"].ToString()) == 1) //This checks if category is a market center company or office
            {
                isMarketCenterCompany = true;
            }
            return isMarketCenterCompany;
        }

        public static bool isCategorySportsTeamCategory(int categoryId) {
            Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
            paramDicEx.Add("@id", categoryId);
            var isSportsTeam = false;
            DBManager manager = new DBManager();
            var dv = manager.GetParameterizedDataView("EXEC usp_SelectTblNopCategory  @id", paramDicEx);
            if (dv.Count > 0 && dv[0]["TypeId"] != DBNull.Value && Int32.Parse(dv[0]["TypeId"].ToString()) == 2) //This checks if category is a sports team
            {
                isSportsTeam = true;
            }
            return isSportsTeam;
        }

        public static bool isCategoryTopLevelMarketCenterCategory(int categoryId) {
            ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
            var category = _categoryService.GetCategoryById(categoryId);
            if (category.ParentCategoryId == 84)
            {
                return true;
            }
            return false;
        }

        private static Uri GetUri(HttpRequest request) {
            var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Value;
            builder.Path = request.Path;
            builder.Query = request.QueryString.ToUriComponent();
            return builder.Uri;
        }

        public static void DeleteCDNCache(List<string> urls = null) {
            IHttpContextAccessor httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            if (urls == null)
            {
                urls = new List<string>();
            }
            urls.Add(GetUri(httpContextAccessor.HttpContext.Request).GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority, UriFormat.Unescaped));
            urls.Add(Helpers.StripQueryStringParam(GetUri(httpContextAccessor.HttpContext.Request).AbsoluteUri, "regenerate"));
            foreach (var url in urls)
            {
                CdnManager.RemoveFromCache(url);
            }
        }

        /// <summary>
        /// Given a URL in any format, return URL with specified query string param removed if it exists
        /// </summary>
        public static string StripQueryStringParam(string url, string paramToRemove) {
            return StripQueryStringParams(url, new List<string> { paramToRemove });
        }

        /// <summary>
        /// Given a URL in any format, return URL with specified query string params removed if it exists
        /// </summary>
        public static string StripQueryStringParams(string url, List<string> paramsToRemove) {
            IHttpContextAccessor httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            if (paramsToRemove == null || !paramsToRemove.Any()) return url;

            var splitUrl = url.Split('?');
            if (splitUrl.Length == 1) return url;

            var urlFirstPart = splitUrl[0];
            var urlSecondPart = splitUrl[1];

            // Even though in most cases # isn't available to context,
            // we may be passing it in explicitly for helper urls
            var secondPartSplit = urlSecondPart.Split('#');
            var querystring = secondPartSplit[0];
            var hashUrlPart = string.Empty;
            if (secondPartSplit.Length > 1)
            {
                hashUrlPart = "#" + secondPartSplit[1];
            }
            var uri = GetUri(httpContextAccessor.HttpContext.Request);
            var nvc = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            if (!nvc.Keys.Any()) return url;

            // Remove any matches
            foreach (var key in nvc.Keys)
            {
                if (paramsToRemove.Contains(key))
                {
                    nvc.Remove(key);
                }
            }

            if (!nvc.Keys.Any()) return urlFirstPart;
            return urlFirstPart +
                   "?" + string.Join("&", nvc.Keys.Select(c => c.ToString() + "=" + nvc[c.ToString()])) +
                   hashUrlPart;
        }
        
        /// <summary>
        /// get market center id from catergory id
        /// </summary>
        public static string getMarketCenterCodeFromCategoryId(int categoryId) {
            DBManager db = new DBManager();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@catId", categoryId);
            var dv = db.GetParameterizedDataView("EXEC usp_getTypeCodeFromCategoryID @catId", param);

            if (dv.Count > 0)
            {
                return dv[0]["typeCode"].ToString();

            }
            return null;
        }
    }


}