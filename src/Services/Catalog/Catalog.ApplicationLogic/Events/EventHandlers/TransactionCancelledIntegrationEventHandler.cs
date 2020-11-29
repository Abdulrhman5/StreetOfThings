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
    public class TransactionCancelledIntegrationEventHandler : IIntegrationEventHandler<TransactionCancelledIntegrationEvent>
    {
        private IRepository<Guid, Transaction> _transRepo;
        private ILogger<TransactionCancelledIntegrationEventHandler> _logger;

        public TransactionCancelledIntegrationEventHandler(IRepository<Guid, Transaction> transRepo,
            ILogger<TransactionCancelledIntegrationEventHandler> logger)
        {
            _transRepo = transRepo;
            _logger = logger;
        }


        async Task IIntegrationEventHandler<TransactionCancelledIntegrationEvent>.HandleEvent(TransactionCancelledIntegrationEvent integrationEvent)
        {
            try
            {
                var theTrans = await (from t in _transRepo.Table
                                      where t.TransactionId == integrationEvent.RegistrationId
                                      select t).SingleOrDefaultAsync();
                theTrans.Status = TransactionStatus.Cancelled;
                await _transRepo.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "There were an error while handling TransactionCancelledIntegrationEventHandler");
            }
            
        }
    }
}
