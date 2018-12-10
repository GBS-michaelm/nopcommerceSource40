using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Payments.GBS;
using Nop.Plugin.Payments.GBS.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nop.Services.Logging;


namespace Nop.Plugin.Payments.GBS.Components
{
    [ViewComponent(Name = "PaymentGBS")]
    public class PaymentGBSViewComponent : NopViewComponent
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
		private readonly ILogger _logger;

		private readonly GBSPaymentSettings _gbsPaymentSettings;

        public PaymentGBSViewComponent(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            GBSPaymentSettings gbsPaymentSettings,
            ILogger logger)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            this._gbsPaymentSettings = _settingService.LoadSetting<GBSPaymentSettings>(storeScope);
            this._logger = logger;
        }

        /// <summary>
        /// Get active store scope (for multi-store configuration mode)
        /// </summary>
        /// <param name="storeService">Store service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Store ID; 0 if we are in a shared mode</returns>
        protected virtual int GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;

            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);

            return store != null ? store.Id : 0;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                //  IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();
                var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);

                int customerID = _workContext.CurrentCustomer.Id;
                ViewBag.UseSavedCards = _gbsPaymentSettings.UseSavedCards;
                ViewBag.StoreID = storeScope;

				if ((_gbsPaymentSettings.UseSavedCards))
				{

					DBManager dbmanager = new DBManager();
					Dictionary<string, string> paramDic = new Dictionary<string, string>();
					paramDic.Add("@CustomerID", customerID.ToString());
					string select = "exec usp_getCCProfiles @CustomerID";
					DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);
					ViewBag.SavedCCCount = dView.Count;



				}

				var model = new PaymentInfoModel();

                //years
                for (var i = 0; i < 15; i++)
                {
                    var year = Convert.ToString(DateTime.Now.Year + i);
                    model.ExpireYears.Add(new SelectListItem
                    {
                        Text = year,
                        Value = year,
                    });
                }

                //months
                for (var i = 1; i <= 12; i++)
                {
                    var text = (i < 10) ? "0" + i : i.ToString();
                    model.ExpireMonths.Add(new SelectListItem
                    {
                        Text = text,
                        Value = i.ToString(),
                    });
                }

                //set postback values
                //var form = HttpContext.Request.Form;
                //if (form != null)
                //{
                //    model.CardholderName = form["CardholderName"];
                //    model.CardNumber = form["CardNumber"];
                //    model.CardCode = form["CardCode"];
                //    var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));

                //    if (selectedMonth != null)
                //        selectedMonth.Selected = true;

                //    var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));

                //    if (selectedYear != null)
                //        selectedYear.Selected = true;
                //}
                return View("~/Plugins/Payments.GBS/Views/PaymentGBS/PaymentInfo.cshtml", model);
				
            }
            catch (Exception ex)
            {
                _logger.Error("error in PaymentInfo()", ex, null);
                throw ex;
            }
        }
    }
}
