using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Models.File;

namespace Nop.Plugin.Order.GBS.Models
{
   public class CanvasProductModel
    {
        public string canvasServerBaseURL { get; set; }
        public string stateID { get; set; }
        public string productType { get; set; }
        public int orderItemID { get; set; }
        public int userID { get; set; }
        public string gbsOrderId { get; set; }
        public string username { get; set; }
        public IList<ProductFileModel> productFileModels { get; set; }

        public CanvasProductModel()
        {
            productFileModels = new List<ProductFileModel>();
        }
        public string ToJSON()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            return output;
        }
        public static CanvasProductModel FromJSON(string input)
        {
            return JsonConvert.DeserializeObject<CanvasProductModel>(input);
        }
    }
}
