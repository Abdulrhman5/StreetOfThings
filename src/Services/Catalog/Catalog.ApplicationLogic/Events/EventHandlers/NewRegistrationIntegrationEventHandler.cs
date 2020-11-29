using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Events.EventHandlers
{
    public class NewRegistrationIntegrationEventHandler : IIntegrationEventHandler<NewRegistrationIntegrationEvent>
    {
        private readonly IRepository<Guid, Transaction> _transactionsRepo;

        private IUserDataManager _userDataManager;

        private ILogger<NewRegistrationIntegrationEventHandler> _logger;
        public NewRegistrationIntegrationEventHandler(IRepository<Guid, Transaction> transactionsRepo, IUserDataManager userDataManager, ILogger<NewRegistrationIntegrationEventHandler> logger)
        {
            _transactionsRepo = transactionsRepo;
            _userDataManager = userDataManager;
            _logger = logger;
        }

        public async Task HandleEvent(NewRegistrationIntegrationEvent integrationEvent)
        {
            var user = await _userDataManager.AddUserIfNeeded(integrationEvent.RecipiantId);
            var transactionModel = new Transaction
            {
                ObjectId = integrationEvent.ObjectId,
                ReceipientId = user.UserId,
                RegisteredAtUtc = integrationEvent.RegisteredAt,
                Status = TransactionStatus.Ok,
                TransactionId = integrationEvent.RegistrationId
            };

            try
            {
                _transactionsRepo.Add(transactionModel);
                await _transactionsRepo.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "A NewRegistrationIntegrationEvent has arraived but couldn't handle it");
            }
        }
    }
}
