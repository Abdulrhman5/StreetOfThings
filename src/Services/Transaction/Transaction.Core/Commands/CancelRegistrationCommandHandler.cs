using EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Extensions;
using Transaction.Core.IntegrationEvents;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;

namespace Transaction.Core.Commands
{
    class CancelRegistrationCommandHandler : IRequestHandler<CancelRegistrationCommand, CommandResult>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IEventBus _eventBus;
        private IOwnershipAuthorization<Guid, ObjectRegistration> _ownershipAuth;

        public CancelRegistrationCommandHandler(IRepository<Guid, ObjectRegistration> registrationsRepo,
            IEventBus eventBus, 
            IOwnershipAuthorization<Guid, ObjectRegistration> ownershipAuth)
        {
            _registrationsRepo = registrationsRepo;
            _eventBus = eventBus;
            _ownershipAuth = ownershipAuth;
        }

        public async Task<CommandResult> Handle(CancelRegistrationCommand request, CancellationToken cancellationToken)
        {
            var registrationIdGuid = Guid.Parse(request.RegistrationId);

            var authResult = _ownershipAuth.IsAuthorized(or => or.ObjectRegistrationId == registrationIdGuid, or => or.RecipientLogin.User);
            if (!authResult)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAUTHORIZED",
                    Message = "You are not authorized to cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand();
            }

            var registrations = from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == registrationIdGuid &&
                                r.ObjectReceiving == null &&
                                r.Status == ObjectRegistrationStatus.OK &&
                                r.ExpiresAtUtc > DateTime.UtcNow
                                select r;

            if (!registrations.Any())
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAVAILABLE",
                    Message = "You can not cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand();
            }


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
