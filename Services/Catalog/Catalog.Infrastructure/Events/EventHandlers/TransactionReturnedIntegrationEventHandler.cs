using EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Entities;

namespace Catalog.Infrastructure.Events.EventHandlers
{
    public class TransactionReturnedIntegrationEventHandler : IIntegrationEventHandler<TransactionReturnedIntegrationEvent>
    {
        private IRepository<Guid, Transaction> _transRepo;
        private ILogger<TransactionReturnedIntegrationEventHandler> _logger;

        public TransactionReturnedIntegrationEventHandler(IRepository<Guid, Transaction> transRepo,
            ILogger<TransactionReturnedIntegrationEventHandler> logger)
        {
            _transRepo = transRepo;
            _logger = logger;
        }


        async Task IIntegrationEventHandler<TransactionReturnedIntegrationEvent>.HandleEvent(TransactionReturnedIntegrationEvent integrationEvent)
        {
            try
            {
                var theTrans = await (from t in _transRepo.Table
                                      where t.TransactionId == integrationEvent.RegistrationId
                                      select t).SingleOrDefaultAsync();
                theTrans.ReturnedAtUtc = integrationEvent.ReturnedAtUtc;
                theTrans.ReturnId = integrationEvent.ReturnIdId;

                await _transRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There were an error while handling TransactionCancelledIntegrationEventHandler");
            }

        }
    }
}
