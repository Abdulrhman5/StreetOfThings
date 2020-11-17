using FluentValidation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;

namespace Transaction.Core.Validations
{
    class CancelRegistrationValidator : AbstractValidator<CancelRegistrationCommand>
    {
        private IOwnershipAuthorization<Guid, ObjectRegistration> _ownershipAuth;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        public CancelRegistrationValidator(IOwnershipAuthorization<Guid, ObjectRegistration> ownershipAuth,
            IRepository<Guid, ObjectRegistration> registrationsRepo)
        {
            _ownershipAuth = ownershipAuth;
            _registrationsRepo = registrationsRepo;

            CascadeMode = CascadeMode.Stop;
            RuleFor(cancelRegistration => cancelRegistration)
                .NotNull()
                .WithMessage("TRANSACTION.REGISTRATION.CANCEL.NULL");

            RuleFor(cancelRegistration => cancelRegistration.RegistrationId)
                .Must(registrationId => Guid.TryParse(registrationId, out var result))
                .WithMessage("TRANSACTION.REGISTRATION.INVALID.DATA");

            RuleFor(cancelRegistration => cancelRegistration.RegistrationId)
                .Must(IsCurrentUserOwensRegistration)
                .WithMessage("TRANSACTION.REGISTRATION.CANCEL.UNAUTHORIZED");

            RuleFor(cancelRegistration => cancelRegistration.RegistrationId)
                .Must(DoesRegistrationExistAndOK)
                .WithMessage("TRANSACTION.REGISTRATION.CANCEL.UNAVAILABLE");

        }

        private bool IsCurrentUserOwensRegistration(string registrationId)
        {
            var guidRegistrationId = Guid.Parse(registrationId);

            var authResult = _ownershipAuth.IsAuthorized(or => or.ObjectRegistrationId == guidRegistrationId, or => or.RecipientLogin.User);

            return authResult;

        }

        private bool DoesRegistrationExistAndOK(string registrationId)
        {
            var guidRegistrationId = Guid.Parse(registrationId);
            var registrations = from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == guidRegistrationId &&
                                r.ObjectReceiving == null &&
                                r.Status == ObjectRegistrationStatus.OK &&
                                r.ExpiresAtUtc > DateTime.UtcNow
                                select r;

            if (!registrations.Any())
            {
                return false;
            }
            return true;
        }
    }

}
