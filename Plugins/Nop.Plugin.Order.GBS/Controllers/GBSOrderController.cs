using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Order.GBS.Models;
using Nop.Plugin.Widgets.CustomersCanvas;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using Nop.Plugin.DataAccess.GBS;
using System.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using WebServices.Models.File;

namespace Nop.Plugin.Order.GBS.Controllers
{
    public class GBSOrderController : BaseController
    {
        private readonly CcSettings _ccSettings;
        private readonly ICcService _ccService;
        private readonly GBSOrderSettings _gbsOrderSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;



        public GBSOrderController(
            ICcService ccService,
            CcSettings ccSettings,
            GBSOrderSettings gbsOrderSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ILogger logger,
            IStoreContext storeContext,
            IPluginFinder pluginFinder)
        {
            this._ccService = ccService;
            this._ccSettings = ccSettings;
            this._gbsOrderSettings = gbsOrderSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._logger = logger;
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
        }

        public ActionResult UpdateCanvasProductView()
        {
            var model = new CanvasProductModel();
            var request = System.Web.HttpContext.Current.Request;
            try
            {
                var miscPlugins = _pluginFinder.GetPlugins<MyOrderServicePlugin>(storeId: _storeContext.CurrentStore.Id).ToList();
                if (miscPlugins.Count > 0)
                {
                    model.orderItemID = Int32.Parse(request.QueryString["orderitemid"]);
                    var productType = request.QueryString["productType"];
                    var webPlatform = request.QueryString["webPlatform"];
                    model.productType = productType;
                    model.webPlatform = webPlatform;

                    IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

                    var customerID = _workContext.CurrentCustomer.Id;
                    model.canvasServerBaseURL = _ccSettings.ServerHostUrl;
                    var tempModel = new CanvasProductModel();
                    string stateID = GetCcStateID(model.orderItemID, productType, out tempModel, webPlatform);

                    model.userID = tempModel.userID;
                    model.username = tempModel.username;
                    model.gbsOrderId = tempModel.gbsOrderId;

                    model.stateID = stateID;
                    //get the product file names and put them in the model.
                    List<ProductFileModel> productFiles = GetProductFiles(model.orderItemID, webPlatform);
                    GBSFileService.GBSFileServiceClient FileService = new GBSFileService.GBSFileServiceClient();
                    string fileServiceaddress = _gbsOrderSettings.GBSPrintFileWebServiceBaseAddress;
                    model.productFileModels = FileService.populateProductFilesFromProductionFileName(productFiles, fileServiceaddress, _gbsOrderSettings.LoginId, _gbsOrderSettings.Password);


                    return View("~/Plugins/Order.GBS/Views/OrderGBS/UpdateCanvasProduct.cshtml", model);
                } else
                {
                    throw new Exception("This store does not have the OrdPlugin installed");
                }
            }
            catch(Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error trying to load UpdateCanvasProductView = orderitemid = " + model.orderItemID, ex, customer);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult CopyFilesToProduction(FormCollection ccFiles)
        {
            try
            {
                List<ProductFileModel> productFiles = JsonConvert.DeserializeObject<List<ProductFileModel>>(ccFiles["FilesToCopy"]);
                //pass to the file service
                GBSFileService.GBSFileServiceClient FileService = new GBSFileService.GBSFileServiceClient();
                string fileServiceaddress = _gbsOrderSettings.GBSPrintFileWebServiceAddress;
                string response = FileService.CopyFilesToProduction(productFiles, fileServiceaddress, _gbsOrderSettings.LoginId, _gbsOrderSettings.Password);
                if (response.Contains("WebServices"))
                {
                    throw new Exception(response);
                }
                return View("~/Plugins/Order.GBS/Views/OrderGBS/UpdateCanvasProductComplete.cshtml");
            }
            catch (Exception ex)
            {
                var customer = _workContext.CurrentCustomer;
                _logger.Error("Error trying to Copy Files to Production for :  " + ccFiles["FilesToCopy"], ex, customer);
                throw ex;
            }
        }
        public string getEnvironment()
        {
            var canvaseBaseUrl = _ccSettings.ServerHostUrl;

            string env = "LIVE";
            if (canvaseBaseUrl.Contains("canvasDev"))
            {
                env = "DEV";
            }else if (canvaseBaseUrl.Contains("canvasTest"))
            {
                env = "TEST";
            }
            else if (canvaseBaseUrl.Contains("canvasStage"))
            {
                env = "STAGE";
            }
            return env;
        }
        public List<ProductFileModel> GetProductFiles(int orderItemID, string webPlatform = "NOP")
        {
            var environment = getEnvironment();
            List<ProductFileModel> productFiles = new List<ProductFileModel>();
            switch (webPlatform)
            {
                case "NOP":
                    {
                        Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                        paramDicEx.Add("@nopOrderItemID", orderItemID);

                        DBManager manager = new DBManager();
                        string select = "EXEC usp_getTblNOPProductionFiles_byOrderItemId @nopOrderItemID";
                        DataView result = manager.GetParameterizedDataView(select, paramDicEx);

                        if (result.Count > 0)
                        {
                            foreach (DataRowView row in result)
                            {
                                ProductFileModel file = new ProductFileModel();
                                file.product.productType = (string)row["ProductType"];
                                file.product.productionFileName = (string)row["FileName"];
                                productFiles.Add(file);
                            }
                        }
                        else
                        {
                            throw new Exception("Can't find production files for order item - " + orderItemID);
                        }
                        break;
                    }

                default:
                    {
                        Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                        paramDicEx.Add("@OPID", orderItemID);

                        DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);
                        string select = "EXEC usp_getCanvasStateFileInfo @OPID";
                        DataView orderItemResult = manager.GetParameterizedDataView(select, paramDicEx);
                        if (orderItemResult.Count > 0)
                        {
                            foreach (DataRowView row in orderItemResult)
                            {
                                ProductFileModel file = new ProductFileModel();
                                file.product.productionFileName = (string)row["frontFile"];
                                productFiles.Add(file);
                                file = new ProductFileModel();
                                file.product.productionFileName = (string)row["backFile"];
                                productFiles.Add(file);
                            }

                        }
                        else
                        {
                            throw new Exception("Can't find order item - " + orderItemID);
                        }

                        break;
                    }
            }
            return productFiles;

        }
        public string GetCcStateID(int orderItemID,  string productType, out CanvasProductModel designModel, string webPlatform = "NOP")
        {
            var environment = getEnvironment();
            string stateID = string.Empty;
            switch (webPlatform)
            {
                case "NOP":
                    {
                        Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                        paramDicEx.Add("@nopOrderItemID", orderItemID);
                        designModel = new CanvasProductModel();

                        DBManager manager = new DBManager();
                        string select = "EXEC usp_getTblNOPOrderItem_byOrderItemId @nopOrderItemID";
                        DataView nopOrderItemResult = manager.GetParameterizedDataView(select, paramDicEx);
                        int ccId = 0;
                        if (nopOrderItemResult.Count > 0)
                        {
                            ccId = (int)nopOrderItemResult[0]["ccId"];
                            designModel.userID = "nopcommerce_"+ nopOrderItemResult[0]["CustomerId"];
                            designModel.username = (string)nopOrderItemResult[0]["Username"];
                            designModel.gbsOrderId = (string)nopOrderItemResult[0]["gbsOrderID"];

                        }
                        else
                        {
                            throw new Exception("Can't find order item - " + orderItemID);
                        }

                        var design = _ccService.GetDesign(ccId);
                        dynamic designData = JsonConvert.DeserializeObject<Object>(design.Data);

                        stateID = designData[productType + "-stateId"];
                        break;
                    }

                default:
                    {
                        Dictionary<string, Object> paramDicEx = new Dictionary<string, Object>();
                        paramDicEx.Add("@OPID", orderItemID);
                        designModel = new CanvasProductModel();

                        DBManager manager = new DBManager(_gbsOrderSettings.HOMConnectionString);
                        string select = "EXEC usp_getCanvasStateFileInfo @OPID";
                        DataView orderItemResult = manager.GetParameterizedDataView(select, paramDicEx);
                        if (orderItemResult.Count > 0)
                        {
                            stateID = (string) orderItemResult[0]["stateID"];
                            designModel.userID = (string) orderItemResult[0]["UserID"];
                            designModel.username = (string) orderItemResult[0]["Username"];
                            designModel.gbsOrderId = (string) orderItemResult[0]["gbsOrderID"];

                        }
                        else
                        {
                            throw new Exception("Can't find order item - " + orderItemID);
                        }

                        break;
                    }
            }


            return stateID;
        }


    }
}
