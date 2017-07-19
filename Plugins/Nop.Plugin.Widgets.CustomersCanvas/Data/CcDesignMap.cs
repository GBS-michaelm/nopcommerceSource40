using System;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;

namespace Nop.Plugin.Widgets.CustomersCanvas.Data
{
    public partial class CcDesignMap : NopEntityTypeConfiguration<CcDesign>
    {
        public CcDesignMap()
        {
            this.ToTable("CcDesign");
            this.HasKey(ccDesign => ccDesign.Id);

            this.Property(p => p.Data);
            this.Property(p => p.ImageUrl);
            this.Property(p => p.DownloadUrlsJson);
        }
    }
}
