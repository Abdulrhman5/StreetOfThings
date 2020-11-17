using EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.IntegrationEvents;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;

namespace Transaction.Core.Commands
{
    class CancelRegistrationCommandHandler : IRequestHandler<CancelRegistrationCommand>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IEventBus _eventBus;

        public CancelRegistrationCommandHandler(IRepository<Guid, ObjectRegistration> registrationsRepo, IEventBus eventBus)
        {
            _registrationsRepo = registrationsRepo;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(CancelRegistrationCommand request, CancellationToken cancellationToken)
        {
            var registrationIdGuid = Guid.Parse(request.RegistrationId);

            var theRegistration = _registrationsRepo.Get(registrationIdGuid);
            theRegistration.Status = ObjectRegistrationStatus.Canceled;
            await _registrationsRepo.SaveChangesAsync();

            _eventBus.Publish(new TransactionCancelledIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                RegistrationId = theRegistration.ObjectRegistrationId,
                CancelledAtUtc = DateTime.UtcNow,
            });

            return default;
        }
    }
}
