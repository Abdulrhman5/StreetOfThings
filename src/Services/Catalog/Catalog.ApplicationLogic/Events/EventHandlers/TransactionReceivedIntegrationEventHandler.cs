using Catalog.DataAccessLayer;
using Catalog.Models;
using EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Catalog.ApplicationLogic.Events.EventHandlers
{
    public class TransactionReceivedIntegrationEventHandler : IIntegrationEventHandler<TransactionReceivedIntegrationEvent>
    {
        private IRepository<Guid, Transaction> _transRepo;

        private ILogger<TransactionReceivedIntegrationEventHandler> _logger;

        public TransactionReceivedIntegrationEventHandler(ILogger<TransactionReceivedIntegrationEventHandler> logger, IRepository<Guid, Transaction> transRepo)
        {
            _logger = logger;
            _transRepo = transRepo;
        }

        public async Task HandleEvent(TransactionReceivedIntegrationEvent integrationEvent)
        {
            try
            {
                var theTrans = await (from t in _transRepo.Table
                                where t.TransactionId == integrationEvent.RegistrationId
                                select t).SingleOrDefaultAsync();

                theTrans.ReceivingId = integrationEvent.ReceivingId;
                theTrans.ReceivedAtUtc = integrationEvent.ReceivedAtUtc;
                await _transRepo.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There were an error while trying to handle TransactionReceivedIntegrationEventHandler");
            }

        }
    }
}
