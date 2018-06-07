using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessDataAccess.GBS;
using Nop.Services.Catalog;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Logging;
using Nop.Services.Customers;
using Nop.Core.Caching;


namespace Nop.Plugin.BusinessLogic.GBS.Factories
{
    public class Helpers
    {

        public static string GetPictureUrl(int catId, string sku = null)
        {
            ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
            IPictureService _pictureService = EngineContext.Current.Resolve<IPictureService>();
            IProductService _productService = EngineContext.Current.Resolve<IProductService>();
            ICacheManager cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");

            string finalResult = null;

            finalResult = cacheManager.Get("GetPictureUrl_"+catId+"_"+sku, 60, () =>
            {
                DBManager manager = new DBManager();
                Dictionary<string, Object> paramDicEx3 = new Dictionary<string, Object>();
                paramDicEx3.Add("@categoryId", catId);
                var select = "EXEC usp_GetClassicImage @categoryId";

                if (!string.IsNullOrEmpty(sku))
                {
                    //If picture exists in picture table return
                    var prodPictColl = _productService.GetProductBySku(sku).ProductPictures;
                    var prodPic = prodPictColl.FirstOrDefault();
                    if (prodPic != null) return _pictureService.GetPictureUrl(prodPic.PictureId);

                    paramDicEx3.Add("@sku", sku);
                    select = "EXEC usp_GetClassicImage @categoryId, @sku";
                }
                else
                {
                    var NopPicture = _pictureService.GetPictureById(_categoryService.GetCategoryById(catId).PictureId);
                    if (NopPicture != null) return _pictureService.GetPictureUrl(NopPicture.Id);
                }

                //If picture exists in Customer's Canvas return
                string canvasUrl = GetCanvasUrl(catId, sku);
                if (!string.IsNullOrEmpty(canvasUrl))
                {
                    return canvasUrl;
                }

                //else go get the legacy image URL
                var result = manager.GetParameterizedScalar(select, paramDicEx3);
                if (result is DBNull)
                {
                    return null;
                }
                return (string)result;
            });
            return finalResult;
        }

        public static string GetCanvasUrl(int catId, string sku = null)
        {
            ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            IProductService _productService = EngineContext.Current.Resolve<IProductService>();
            ICcService _ccService = EngineContext.Current.Resolve<ICcService>();
            ILogger _logger = EngineContext.Current.Resolve<ILogger>();

            try
            {
                string canvasURL = null;

                DBManager manager = new DBManager();
                Dictionary<string, Object> paramDicEx3 = new Dictionary<string, Object>();
                paramDicEx3.Add("@categoryId", catId);
                var select = "EXEC usp_GetMarketCenterProductJson @categoryId";
                if (!string.IsNullOrEmpty(sku))
                {
                    paramDicEx3.Add("@sku", sku);
                    select = "EXEC usp_GetMarketCenterProductJson @categoryId, @sku";
                }
                var result = manager.GetParameterizedDataView(select, paramDicEx3);

                string jsonData = null;
                if (result.Count > 0)
                {
                    DataRow resultRow = result.Table.Rows[0];
                    jsonData = resultRow["EditorJson"].ToString();
                }
                if (!string.IsNullOrEmpty(jsonData))
                {
                    //get template name and make Canvas API call to get the URL.
                    dynamic apiJson = new ExpandoObject();
                    string previewOptons = "{ 'width': 600, 'height': 600, 'resizeMode': 'Fit', 'proofImageRendering': { 'showStubContent': true } }";

                    int productId = int.MinValue;

                    if (!string.IsNullOrEmpty(sku))
                    {
                        productId = _productService.GetProductBySku(sku).Id;
                    }
                    else
                    {
                        //if no sku then find featured product or return null if not exists
                        paramDicEx3 = new Dictionary<string, Object>();
                        paramDicEx3.Add("@categoryId", catId);
                        select = "EXEC usp_SelectGBSCustomCategoryData @categoryId";
                        var catResult = manager.GetParameterizedDataView(select, paramDicEx3);
                        if (catResult.Count > 0)
                        {
                            DataRow resultRow = catResult.Table.Rows[0];
                            if (!int.TryParse(resultRow["FeaturedProductId"].ToString(), out productId))
                            {
                                return null;
                            }
                        }
                        if (productId <= 0)
                        {
                            return null;
                        }
                    }

                    //go get the template from specification attribute
                    var specAttrs = specService.GetProductSpecificationAttributes(productId);

                    string template = null;
                    var specAttr = specAttrs.Where(x => x.SpecificationAttributeOption.SpecificationAttribute.Name == "Front Template").FirstOrDefault();
                    if (specAttr != null)
                    {
                        template = specAttr.CustomValue;
                    }
                    if (string.IsNullOrEmpty(template))
                    {
                        return null;
                    }
                    string productDefinitions = "[{ 'surfaces': ['" + template + "'] }]";
                    productDefinitions = productDefinitions.Replace(@"\", @"/");


                    apiJson.previewOptions = JsonConvert.DeserializeObject<object>(previewOptons);
                    apiJson.itemsData = JsonConvert.DeserializeObject<object>(jsonData);
                    apiJson.productDefinitions = JsonConvert.DeserializeObject<object>(productDefinitions);
                    apiJson.userId = "default";

                    //Make API call
                    string apiJsonString = JsonConvert.SerializeObject(apiJson);
                    var apiURL = _ccService.ServerHostUrl() + "/api/Preview/GeneratePreview";
                    WebClient client = new WebClient();
                    client.Headers.Add("Content-Type", "application/json");
                    var CustomersCanvasSecurityKey = System.Configuration.ConfigurationManager.AppSettings["CustomersCanvasApiSecurityKey"];
                    client.Headers.Add("X-CustomersCanvasAPIKey", CustomersCanvasSecurityKey);

                    string responseString = client.UploadString(apiURL, apiJsonString);
                    List<dynamic> responseList = JsonConvert.DeserializeObject<List<Object>>(responseString);
                    canvasURL = responseList.FirstOrDefault()[0][0].ToString();
                }

                return canvasURL;
            }catch (Exception ex)
            {
                _logger.Error("Error generating Canvas URL in GetCanvasUrl().  CategoryId = "+catId+", Sku = "+sku, ex);

                return null;
            }
        }
    }
}