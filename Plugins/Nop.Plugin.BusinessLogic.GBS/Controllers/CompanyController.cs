using Nop.Web.Framework.Controllers;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.BusinessLogic.GBS.Domain;
using Nop.Web.Controllers;
using Nop.Plugin.BusinessLogic.GBS.Models;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Plugin.BusinessLogic.GBS;
using Nop.Services.Configuration;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.BusinessLogic.GBS.Controllers
{
    public class CompanyController : BaseController
    {
        //testing homegate 4725

        //actions for querying and then sending data to view
        public ActionResult CanvasNameBadge(int companyId)
        {
            //*****Still needs infrastructure refs********
            CompanyProduct companyProduct = new CompanyProduct();
            CompanyProductsModel companyProductsListForView = new CompanyProductsModel();
            List<CompanyProduct> companyProducts = companyProduct.GetProductsAndFields(companyId);

            for(int i = 0; i < companyProducts.Count; i++)
            {
                companyProductsListForView.productList.Add(companyProducts[i]);
            }
            //use companyProducts View that will hold companyProduct
            
            return View("CompanyProducts", companyProductsListForView);

        }
        
    }
}
