using Nop.Core.Data;
using Nop.Services.Events;
using Nop.Plugin.Widgets.MarketCenters.GBS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Services
{
    public class MarketCentersService : IMarketCentersService
    {
        #region Fields
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<MC_Client> _clientRepository;
        #endregion

        public MarketCentersService(IEventPublisher eventPublisher,
           IRepository<MC_Client> clientRepository)
        {
            this._eventPublisher = eventPublisher;
            this._clientRepository = clientRepository;            
        }

        public virtual IEnumerable<MC_Client> GetClients()
        {
            return _clientRepository.Table;
        }

        public virtual MC_Client GetClientById(int Id)
        {
            return _clientRepository.Table.FirstOrDefault(s=> s.Id == Id);
        }

        public virtual void InsertClient(MC_Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _clientRepository.Insert(client);

            //event notification
            _eventPublisher.EntityInserted(client);
        }

        public virtual void UpdateClient(MC_Client client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            _clientRepository.Update(client);

            //event notification
            _eventPublisher.EntityUpdated(client);
        }
    }
}
