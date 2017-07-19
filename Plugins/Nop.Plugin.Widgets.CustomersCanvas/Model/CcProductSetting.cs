using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcProductSetting
    {
        public int ProductId { get; set; }

        public string CcUrl { get; set; }
        public int EditorSpecAttrId { get; set; }
        public int EditorSpecAttrOptionId { get; set; }

        public int EditorConfigSpecAttrId { get; set; }
        public int EditorConfigSpecAttrOptionId { get; set; }

        public List<EditorData> EditorList { get; set; }
        

        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.CcEditor.SelectFromList")]
        public string EditorName { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.CustomersCanvas.CcEditorConfig.SelectFromList")]
        public string ConfigName { get; set; }

        public CcProductSetting()
        {

        }

    }
}
