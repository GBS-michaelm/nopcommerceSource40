using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Catalog.GBS.Seo
{
    public class GenericPathRoute : Nop.Web.Framework.Seo.GenericPathRoute
    {
        public GenericPathRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler)
        {
        }

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data != null && DataSettingsHelper.DatabaseIsInstalled())
            {
                var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
                var slug = data.Values["generic_se_name"] as string;
                //performance optimization.
                //we load a cached verion here. it reduces number of SQL requests for each page load
                var urlRecord = urlRecordService.GetBySlugCached(slug);
                //comment the line above and uncomment the line below in order to disable this performance "workaround"
                //var urlRecord = urlRecordService.GetBySlug(slug);
                if (urlRecord == null)
                {
                    //no URL record found

                    //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    //var response = httpContext.Response;
                    //response.Status = "302 Found";
                    //response.RedirectLocation = webHelper.GetStoreLocation(false);
                    //response.End();
                    //return null;

                    data.Values["controller"] = "Common";
                    data.Values["action"] = "PageNotFound";
                    return data;
                }
                //ensre that URL record is active
                if (!urlRecord.IsActive)
                {
                    //URL record is not active. let's find the latest one
                    var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                    if (!string.IsNullOrWhiteSpace(activeSlug))
                    {
                        //the active one is found
                        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        var response = httpContext.Response;
                        response.Status = "301 Moved Permanently";
                        response.RedirectLocation = string.Format("{0}{1}", webHelper.GetStoreLocation(false), activeSlug);
                        response.End();
                        return null;
                    }
                    else
                    {
                        //no active slug found

                        //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        //var response = httpContext.Response;
                        //response.Status = "302 Found";
                        //response.RedirectLocation = webHelper.GetStoreLocation(false);
                        //response.End();
                        //return null;

                        data.Values["controller"] = "Common";
                        data.Values["action"] = "PageNotFound";
                        return data;
                    }
                }

                //ensure that the slug is the same for the current language
                //otherwise, it can cause some issues when customers choose a new language but a slug stays the same
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var slugForCurrentLanguage = SeoExtensions.GetSeName(urlRecord.EntityId, urlRecord.EntityName, workContext.WorkingLanguage.Id);
                if (!String.IsNullOrEmpty(slugForCurrentLanguage) &&
                    !slugForCurrentLanguage.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                {
                    //we should make not null or "" validation above because some entities does not have SeName for standard (ID=0) language (e.g. news, blog posts)
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    var response = httpContext.Response;
                    //response.Status = "302 Found";
                    response.Status = "302 Moved Temporarily";
                    response.RedirectLocation = string.Format("{0}{1}", webHelper.GetStoreLocation(false), slugForCurrentLanguage);
                    response.End();
                    return null;
                }

                //process URL
                if (urlRecord.EntityName.ToLowerInvariant() == "category")
                {

                    data.Values["controller"] = "Catalog";
                    data.Values["action"] = "Category";
                    data.Values["categoryid"] = urlRecord.EntityId;
                    data.Values["SeName"] = urlRecord.Slug;
                    data.DataTokens["Namespaces"] = ((string[])data.DataTokens["Namespaces"]).Where((source, index) => source != "Nop.Web.Controllers").ToArray();


                }

            }
            return data;
        }
    }
}
