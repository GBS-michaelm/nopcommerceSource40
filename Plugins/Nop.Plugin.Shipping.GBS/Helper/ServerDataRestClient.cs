using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Plugin.Shipping.GBS.Models.Product;
using RestSharp;
using System.Configuration;
using System.Net;
using Nop.Services.Localization;

namespace Nop.Plugin.Shipping.GBS.Helper
{
    public class ServerDataRestClient : IServerDataRestClient
    {
        private readonly ILocalizationService _localizationService ;

        private readonly RestClient _client;

        //getting base url from web.config  
        private readonly string _url = Properties.Settings.Default.GBSWebApiBaseUrl;     

        #region Corn

        public ServerDataRestClient(ILocalizationService localizationService)
        {
            this._client = new RestClient { BaseUrl = new Uri(_url) };
            this._localizationService = localizationService;
        }

        #endregion

       
        #region Shipping Category 
        public string AddShippingCategory(ProductGBSModel ProductModel)
        {
           var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Add.Shipping.Category.WebApi.Url");
           var _parameterName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Add.Shipping.Category.Parameter.Name");

            var request = new RestRequest(_webapiurl.ResourceValue, Method.POST) { RequestFormat = DataFormat.Json }; //Product /{ id}
            request.AddParameter( _parameterName.ResourceValue, ProductModel.ProductID, ParameterType.UrlSegment); //id
            request.AddBody(ProductModel);

            var response = _client.Execute<ProductGBSModel>(request);

            //if (response.StatusCode == HttpStatusCode.NotFound)
            //    throw new Exception(response.ErrorMessage);

            return response.StatusCode.ToString();
        }
        public string UpdateShippingCategory(ProductGBSModel ProductModel)
        {
            var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Update.Shipping.Category.WebApi.Url");
            var _parameterName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Update.Shipping.Category.Parameter.Name");

            var request = new RestRequest(_webapiurl.ResourceValue, Method.PUT) { RequestFormat = DataFormat.Json }; //Product /{ id}
            request.AddParameter(_parameterName.ResourceValue, ProductModel.ProductID, ParameterType.UrlSegment); //id
            request.AddBody(ProductModel);

            var response = _client.Execute<ProductGBSModel>(request);
            //if (response.StatusCode == HttpStatusCode.NotFound)
            //    throw new Exception(response.ErrorMessage);

            return response.StatusCode.ToString();
        }

        public IEnumerable<ProductGBSModel> GetAllShippingCategories()
        {
            var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.All.Shipping.Categories.WebApi.Url");

            var request = new RestRequest(_webapiurl.ResourceValue, Method.GET) { RequestFormat = DataFormat.Json }; // Product 

            var response = _client.Execute<List<ProductGBSModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public ProductGBSModel GetShippingCategoryByID(int id)
        {
            var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.Shipping.Category.By.Id.WebApi.Url");
            var _parameterName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.Shipping.Category.By.Id.Parameter.Name");

            var request = new RestRequest(_webapiurl.ResourceValue, Method.GET) { RequestFormat = DataFormat.Json }; // Product/{id}
            request.AddParameter(_parameterName.ResourceValue, id, ParameterType.UrlSegment); // id

            var response = _client.Execute<ProductGBSModel>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            return response.Data;
        }

        #endregion

        #region Shiping Class

        //public IEnumerable<ShippingClassModel> GetAllShippingClasses()
        //{
        //    var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.All.Shipping.Classes.WebApi.Url");

        //    var request = new RestRequest( _webapiurl.ResourceValue , Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<ShippingClassModel>>(request);

        //    if (response.Data == null)
        //        return null;

        //    return response.Data;
        //}

        //public ShippingClassModel GetShippingClassByID(int id)
        //{
        //    var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.Shipping.Class.By.Id.WebApi.Url");
        //    var _parameterName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.Shipping.Class.By.Id.Parameter.Name");

        //    var request = new RestRequest( _webapiurl.ResourceValue , Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter(_parameterName.ResourceValue , id, ParameterType.UrlSegment);

        //    var response = _client.Execute<ShippingClassModel>(request);

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //        return null;

        //    return response.Data;
        //}

        #endregion

        #region Zip to Ship

        //public ZipToShipModel GetByZipCode(string zipCode)
        //{
        //    var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.ZipCode.By.ZipCode.WebApi.Url");
        //    var _parameterName = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.ZipCode.By.ZipCode.Parameter.Name");

        //    var request = new RestRequest(_webapiurl.ResourceValue , Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter(_parameterName.ResourceValue , zipCode, ParameterType.UrlSegment);

        //    var response = _client.Execute<ZipToShipModel>(request);

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //        return null;

        //    return response.Data;
        //}
        //public IEnumerable<ZipToShipModel> GetAllZipCodes()
        //{
        //    var _webapiurl = _localizationService.GetLocaleStringResourceByName("Plugins.Shipping.GBS.Product.Get.All.ZipCodes.WebApi.Url");

        //    var request = new RestRequest( _webapiurl.ResourceValue , Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<ZipToShipModel>>(request);

        //    if (response.Data == null)
        //        return null;

        //    return response.Data;
        //}
        #endregion



    
    }
}