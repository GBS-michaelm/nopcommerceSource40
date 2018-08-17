using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

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

        public CcProductSetting() { }

    }
}
