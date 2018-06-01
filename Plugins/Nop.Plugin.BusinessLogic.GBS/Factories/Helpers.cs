using Nop.Plugin.BusinessDataAccess.GBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.BusinessLogic.GBS.Factories
{
    public class Helpers
    {
        public static string GetPictureUrl(int catId, string sku = null)
        {
            DBManager manager = new DBManager();
            Dictionary<string, Object> paramDicEx3 = new Dictionary<string, Object>();
            paramDicEx3.Add("@categoryId", catId);
            var select = "EXEC usp_GetClassicImage @categoryId";

            if (!string.IsNullOrEmpty(sku))
            {
                paramDicEx3.Add("@sku", sku);
                select = "EXEC usp_GetClassicImage @categoryId, @sku";
            }

           
            var result = manager.GetParameterizedScalar(select, paramDicEx3);
            if (result is DBNull)
            {
                return null;
            }
            return (string)result;
        }
    }
}
