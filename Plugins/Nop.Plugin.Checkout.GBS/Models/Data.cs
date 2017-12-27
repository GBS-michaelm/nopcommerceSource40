using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Checkout.GBS.Models
{

    public class ImageData
    {
    }

    public class SubmitItemResponse
    {
        public string status { get; set; } = "success";
        public string message { get; set; } = "";
        public IList<string> warnings { get; set; } = new List<string>();
    }
}

