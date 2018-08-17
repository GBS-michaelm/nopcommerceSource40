using Nop.Data.Mapping;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;

namespace Nop.Plugin.Widgets.CustomersCanvas.Data
{
    public partial class CcDesignMap : NopEntityTypeConfiguration<CcDesign>
    {
        public CcDesignMap()
        {
            ToTable("CcDesign");
            HasKey(ccDesign => ccDesign.Id);

            Property(p => p.Data);
            Property(p => p.ImageUrl);
            Property(p => p.DownloadUrlsJson);
        }
    }
}
