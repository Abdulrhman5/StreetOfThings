using CommonLibrary;
using EventBus;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Events;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class ObjectRegistrationCanceller
    {
        private readonly IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private IEventBus _eventBus;

        private OwnershipAuthorization<Guid, ObjectRegistration> _ownershipAuth;

        private readonly IRepository<int, OfferedObject> _objectRepo;
        public ObjectRegistrationCanceller(IRepository<Guid, ObjectRegistration> registrationsRepo,
            IEventBus eventBus,
            OwnershipAuthorization<Guid, ObjectRegistration> ownershipAuth)
        {
            _registrationsRepo = registrationsRepo;
            _eventBus = eventBus;
            _ownershipAuth = ownershipAuth;
        }

        public async Task<CommandResult> CancelRegistration(CancelRegistrationTokenDto cancelRegistration)
        {
            if(cancelRegistration == null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!Guid.TryParse(cancelRegistration.RegistrationId, out var registrationIdGuid))
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.INVALID.DATA",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var authResult = _ownershipAuth.IsAuthorized(or => or.ObjectRegistrationId == registrationIdGuid, or => or.RecipientLogin.User);
            if (!authResult)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAUTHORIZED",
                    Message = "You are not authorized to cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var registrations = from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == registrationIdGuid &&
                                r.ObjectReceiving == null &&
                                r.Status == ObjectRegistrationStatus.OK &&
                                r.ExpiresAtUtc > DateTime.UtcNow
                                select r;

            if(!registrations.Any())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAVAILABLE",
                    Message = "You can not cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
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
            return new CommandResult();
        }
    }

    public class CancelRegistrationTokenDto
    {
        public string RegistrationId { get; set; }
    }
}
