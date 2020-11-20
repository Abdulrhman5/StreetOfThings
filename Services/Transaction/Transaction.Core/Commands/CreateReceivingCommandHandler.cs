using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;
using Transaction.Core.IntegrationEvents;
using EventBus;

namespace Transaction.Core.Commands
{
    class CreateReceivingCommandHandler : IRequestHandler<CreateReceivingCommand, CreateReceivingResultDto>
    {
        private IRepository<Guid, TransactionToken> _tokensRepo;
        private IUserDataManager userDataManager;
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;
        private IEventBus _eventBus;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IRepository<int, OfferedObject> _objectRepo;

        public CreateReceivingCommandHandler(IRepository<Guid, TransactionToken> tokensRepo, 
            IUserDataManager userDataManager, 
            IRepository<Guid, ObjectReceiving> receivingsRepo, 
            IEventBus eventBus, 
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IRepository<int, OfferedObject> objectRepo)
        {
            _tokensRepo = tokensRepo;
            this.userDataManager = userDataManager;
            _receivingsRepo = receivingsRepo;
            _eventBus = eventBus;
            _registrationsRepo = registrationsRepo;
            _objectRepo = objectRepo;
        }

        public async Task<CreateReceivingResultDto> Handle(CreateReceivingCommand request, CancellationToken cancellationToken)
        {
            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();

            var theToken = (from t in _tokensRepo.Table
                            where t.Token == request.RegistrationToken && t.Type == TokenType.Receiving
                            select t).FirstOrDefault();


            var theRegistration = (from rg in _registrationsRepo.Table
                                   where rg.Tokens.Any(rt => rt.Token == request.RegistrationToken)
                                   select rg).FirstOrDefault();

            theToken.Status = TokenStatus.Used;

            var receiving = new ObjectReceiving
            {
                ReceivedAtUtc = DateTime.UtcNow,
                GiverLoginId = login.LoginId,
                RecipientLoginId = theToken.IssuerLoginId,
                HourlyCharge = 0f,
                ObjectRegistrationId = theRegistration.ObjectRegistrationId,
                ObjectReceivingId = Guid.NewGuid(),
            };

            _receivingsRepo.Add(receiving);
            // this will save theToken.Status also
            await _receivingsRepo.SaveChangesAsync();

            var evnt = new TransactionReceivedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                ReceivingId = receiving.ObjectReceivingId,
                RegistrationId = receiving.ObjectRegistrationId,
            };
            _eventBus.Publish(evnt);

            // Publish the event
            return new CreateReceivingResultDto
            {
                ObjectId = _objectRepo.Get(theRegistration.ObjectId).OriginalObjectId,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                RegistrationId = theRegistration.ObjectRegistrationId,
                ShouldBeReturnedAfterReceving = theRegistration.ShouldReturnItAfter,
            };
        }
    }
}
