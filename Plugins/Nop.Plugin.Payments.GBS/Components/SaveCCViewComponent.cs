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
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Payments.GBS.Components
{
    [ViewComponent(Name = "SaveCCView")]
    public class SaveCCViewComponent : NopViewComponent
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
		private readonly ILogger _logger;

		private readonly GBSPaymentSettings _gbsPaymentSettings;

        public SaveCCViewComponent(ILocalizationService localizationService,
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
            var model = new CustomerPaymentProfilesModel();

            IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

            model.customerID = _workContext.CurrentCustomer.Id;

            DBManager dbmanager = new DBManager();
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("@CustomerID", model.customerID.ToString());
            string select = "SELECT * FROM Profiles WHERE CustomerID = " + model.customerID + "";
            DataView dView = dbmanager.GetParameterizedDataView(select, paramDic);  //dbmanager.GetDataView(select);

			if (dView != null)
			{
				if (dView.Count > 0)
				{
					foreach (DataRow dRow in dView.Table.Rows)
					{
						PaymentMethodModel getProfile = new PaymentMethodModel();
						getProfile.profileID = (int)dRow["ProfileID"];
						getProfile.NickName = dRow["NickName"].ToString();
						getProfile.Last4Digits = dRow["Last4Digits"].ToString();
						getProfile.CardType = dRow["CardType"].ToString();
						getProfile.ExpMonth = (int)dRow["ExpMonth"];
						getProfile.ExpYear = (int)dRow["ExpYear"];
						model.SavedProfiles.Add(getProfile);
					}
				}
			}

            return View("~/Plugins/Payments.GBS/Views/PaymentGBS/SaveCreditCard.cshtml", model);
        }
    }
}
