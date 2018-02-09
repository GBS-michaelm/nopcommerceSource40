using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Login.GBS.Models
{
    public class ResponseModel
    {
        public string status { get; set; } = "success";
        public string message { get; set; } = "";
        public IList<string> warnings { get; set; } = new List<string>();
    }
}
