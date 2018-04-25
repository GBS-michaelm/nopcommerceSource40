using Nop.Data.Mapping;
using Nop.Plugin.Widgets.MarketCenters.GBS.Domain;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Data
{
    public partial class MC_ClientMap : NopEntityTypeConfiguration<MC_Client>
    {
        public MC_ClientMap()
        {
            this.ToTable("MC_Client");
            this.HasKey(tr => tr.Id);            
        }
    }
}