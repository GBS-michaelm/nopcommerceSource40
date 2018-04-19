using Nop.Plugin.Catalog.GBS.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Nop.Plugin.Catalog.GBS.HtmlHelpers
{
    public static class CategoryHtmlExtensions
    {
        public static MvcHtmlString FastCategoryListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            DBManager manager = new DBManager();
            Dictionary<string, Object> paramDicEx3 = new Dictionary<string, Object>();
            Dictionary<string, string> parameterTypes = new Dictionary<string, string>();
            DataTable selectedIdsTable = new DataTable();
            selectedIdsTable.Columns.Add("integers",typeof(int));

            ModelMetadata fieldmetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var selectedIds = (List<int>)fieldmetadata.Model;

            foreach (var id in selectedIds)
            {
                selectedIdsTable.Rows.Add(id);
            }
            //Func<TModel, TProperty> deleg = expression.Compile();
            //var result = deleg(htmlHelper.ViewData.Model);


            DataTable selectCatList = new DataTable();
            selectCatList.Columns.Add("Text", typeof(string));
            selectCatList.Columns.Add("Value", typeof(int));
            foreach (var item in selectList)
            {
                selectCatList.Rows.Add(item.Text, Int32.Parse(item.Value));

            }

            paramDicEx3.Add("@tvpSelectedCategoryIds", selectedIdsTable);
            parameterTypes.Add("@tvpSelectedCategoryIds", "dbo.IntegerListTableType");

            paramDicEx3.Add("@tvpCategoryList", selectCatList);
            parameterTypes.Add("@tvpCategoryList", "dbo.SelectListType");

            var select = "EXEC usp_GetCategoryHtmlSelectList @tvpSelectedCategoryIds, @tvpCategoryList";

            var result = manager.GetParameterizedScalar(select, paramDicEx3, parameterTypes);
            if (result is DBNull)
            {
                return null;
            }
            return MvcHtmlString.Create((string)result);
        }


    }
}
