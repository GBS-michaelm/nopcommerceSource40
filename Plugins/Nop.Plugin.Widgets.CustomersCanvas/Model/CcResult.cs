using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CustomersCanvas.Data
{
    public class CcResult
    {
        public string[] HiResUrls;

        public string StateId;

        public string[] ProofUrls;

        public string HiResOutputName;

        public string ReturnToEditUrl;
    }

    public enum CcProductAttributes
    {
        CcId
    }
    public enum CcAttributes
    {
        HiResUrls,
        ProofUrls,
        StateId,
        ReturnToEditUrl
    }


}
