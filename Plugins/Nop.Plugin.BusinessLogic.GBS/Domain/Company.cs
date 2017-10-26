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
       
        private int _categoryId = 0;
        private int _parentCategoryId = 0;
        private string _name = "";
        private string _h1 = "";
        private string _h2 = "";
        private string _backgroundColor = "";
        private bool _isCompany = false;
        private string _logoPicturePath = "";
        private string _landingHeader = "";
        private string _aboutYourMarketCenter = "";
        private int _statusId = 0;
        private string _forgroundColor = "#000000";        
        private string _description = "";

        public Company(string companyId)
        {
            //datalook up via company id
            Dictionary<string, string> companyDic = new Dictionary<string, string>();
            companyDic.Add("@CategoryId", companyId);

            string companyDataQuery = "EXEC usp_SelectGBSCompanyData @CategoryId";
            DataView companyDataView = manager.GetParameterizedDataView(companyDataQuery, companyDic);

            if(companyDataView.Count > 0)
            {
                this.categoryId = Int32.Parse(companyDataView[0]["CategoryID"].ToString());
                int parentCatId;
                this.parentCategoryId = Int32.TryParse(companyDataView[0]["ParentCategoryID"].ToString(), out parentCatId) ? parentCatId : _parentCategoryId;
                this.name = !string.IsNullOrEmpty(companyDataView[0]["Name"].ToString()) ? companyDataView[0]["Name"].ToString() : _name;
                this.h1 = !string.IsNullOrEmpty(companyDataView[0]["H1"].ToString()) ? companyDataView[0]["H1"].ToString() : this.name;
                this.h2 = !string.IsNullOrEmpty(companyDataView[0]["H2"].ToString()) ? companyDataView[0]["H2"].ToString() : _h2;
                this.backgroundColor = !string.IsNullOrEmpty(companyDataView[0]["BackgroundColor"].ToString()) ? companyDataView[0]["BackgroundColor"].ToString() : _backgroundColor;
                this.isCompany = (bool)companyDataView[0]["IsCompany"];
                this.logoPicturePath = !string.IsNullOrEmpty(companyDataView[0]["LogoPicturePath"].ToString()) ? companyDataView[0]["LogoPicturePath"].ToString() : _logoPicturePath;
                this.landingHeader = !string.IsNullOrEmpty(companyDataView[0]["LandingHeader"].ToString()) ? companyDataView[0]["LandingHeader"].ToString() : _landingHeader;
                this.aboutYourMarketCenter = !string.IsNullOrEmpty(companyDataView[0]["aboutYourMarketCenter"].ToString()) ? companyDataView[0]["aboutYourMarketCenter"].ToString() : _aboutYourMarketCenter;
                int statId;
                this.statusId = Int32.TryParse(companyDataView[0]["StatusID"].ToString(), out statId) ? statId : _statusId;
                this.foregroundColor = !string.IsNullOrEmpty(companyDataView[0]["ForegroundColor"].ToString()) ? companyDataView[0]["ForegroundColor"].ToString() : _forgroundColor;           
                this.description = !string.IsNullOrEmpty(companyDataView[0]["Description"].ToString()) ? companyDataView[0]["Description"].ToString() : _description;             
            }

        }
        
        public int categoryId { get { return _categoryId; } set { _categoryId = value; } }
        public int parentCategoryId { get { return _parentCategoryId; } set { _parentCategoryId = value; } }
        public string name { get { return _name; } set { _name = value; } }
        public string h1 { get { return _h1; } set { _h1 = value; } }
        public string h2 { get { return _h2; } set { _h2 = value; } }
        public string backgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public bool isCompany { get { return _isCompany; } set { _isCompany = value; } }
        public string logoPicturePath { get { return _logoPicturePath; } set { _logoPicturePath = value; } }
        public string landingHeader { get { return _landingHeader; } set { _landingHeader = value; } }
        public string aboutYourMarketCenter { get { return _aboutYourMarketCenter; } set { _aboutYourMarketCenter = value; } }
        public int statusId { get { return _statusId; } set { _statusId = value; } }
        public string foregroundColor { get { return _forgroundColor; } set { _forgroundColor = value; } }
        public string description { get { return _description; } set { _description = value; } }       

    }
}
