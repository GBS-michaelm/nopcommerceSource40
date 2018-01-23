using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcEditorLoaderModel
    {
        public CcSettings PluginSettings { get; set; }
        public int ProductId { get; set; }
        public ProductDetailsModel ProductDetails { get; set; }
        public string EditorFolder { get; set; }
        public string Path { get; set; }
        public string Editor { get; set; }
        public string Config { get; set; }
        public int UpdateCartItemId { get; set; }
        public CcDesign CcData { get; set; }
        public int Quantity { get; set; }
        public UserModel User { get; set; }
        public string Language { get; set; }
    }

    public class UserModel
    {
        public int UserId { get; set; }
        public string UserGuid { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class EditorPageModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UpdateCartItemId { get; set; }
    }
}
