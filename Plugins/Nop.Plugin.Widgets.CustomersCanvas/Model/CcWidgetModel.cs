using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Web.Models.Catalog;

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
        public CurrencyModel Currency {get;set;}
    }

    public class CurrencyModel
    {
        public string Code { get; set; }
        public string Locale { get; set; }
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
