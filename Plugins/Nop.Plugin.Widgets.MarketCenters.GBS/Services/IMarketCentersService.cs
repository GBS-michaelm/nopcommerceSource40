using Nop.Plugin.Widgets.MarketCenters.GBS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Services
{
    public interface IMarketCentersService
    {
        IEnumerable<MC_Client> GetClients();
        MC_Client GetClientById(int Id);
        void InsertClient(MC_Client client);
        void UpdateClient(MC_Client client);

    }
}
