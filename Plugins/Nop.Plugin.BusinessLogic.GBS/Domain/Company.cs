using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.BusinessDataAccess.GBS;
using System.Data;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
    public class Company
    {

        DBManager manager = new DBManager();
        

        public Company(string companyId)
        {
            //datalook up via company id
            Dictionary<string, string> categoryDic = new Dictionary<string, string>();
            categoryDic.Add("@CategoryId", companyId);

            string categoryDataQuery = "EXEC usp_SelectGBSCustomCategoryData @CategoryId";
            DataView categoryDataView = manager.GetParameterizedDataView(categoryDataQuery, categoryDic);

            if(categoryDataView.Count > 0)
            {
                this.h1 = categoryDataView[0]["H1"].ToString();
                this.h2 = categoryDataView[0]["H2"].ToString();
                this.foregroundColor = categoryDataView[0]["ForegroundColor"].ToString();
                this.aboutYourMarketCenter = categoryDataView[0]["aboutYourMarketCenter"].ToString();
            }

        }
        
        public string h1 { get; set; } = "";
        public string h2 { get; set; } = "";
        public bool isCompany { get; set; } = true;
        public string aboutYourMarketCenter { get; set; } = "";

        public string foregroundColor { get; set; } = "#000000";
                  

    }
}
