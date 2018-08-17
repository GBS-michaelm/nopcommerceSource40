using Nop.Core;

namespace Nop.Plugin.Widgets.CustomersCanvas.Domain
{
    public class CcDesign : BaseEntity
    { 
        public string Data { get; set; }

        public string ImageUrl { get; set; }

        public string DownloadUrlsJson { get; set; }
    }
}
