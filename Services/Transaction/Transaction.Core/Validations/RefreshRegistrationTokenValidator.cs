using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using System.Linq;
using Transaction.Domain.Entities;

namespace Transaction.Core.Validations
{
    class RefreshRegistrationTokenValidator : AbstractValidator<RefreshRegistrationTokenCommand>
    {
        private IUserDataManager _userDataManager;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        public RefreshRegistrationTokenValidator(IUserDataManager userDataManager,
            IRepository<Guid, ObjectRegistration> registrationsRepo)
        {
            _userDataManager = userDataManager;
            _registrationsRepo = registrationsRepo;


            RuleFor(refreshToken => refreshToken)
                .NotNull()
                .WithMessage("TRANSACTION.REGISTRATION.REFRESH.NULL");

            RuleFor(refreshToken => refreshToken)
                .Must(r => _userDataManager.GetCurrentUser() is object)
                .WithMessage("TRANSACTION.REGISTRATION.REFRESH.USER.UNKOWN");

            RuleFor(refreshToken => refreshToken.ObjectId)
                .Must(IsRegistrationExistAndValide)
                .WithMessage("TRANSACTION.REGISTRATION.REFRESH.NOT.VALID");
        }

        private bool IsRegistrationExistAndValide(int objectId)
        {
            var user = _userDataManager.GetCurrentUser();
            var registrations = from r in _registrationsRepo.Table
                                where r.Object.OriginalObjectId == objectId && r.RecipientLogin.UserId == Guid.Parse(user.UserId) &&
                                r.Status == ObjectRegistrationStatus.OK && r.ExpiresAtUtc > DateTime.UtcNow && r.ObjectReceiving == null
                                select r;

            if (!registrations.Any())
            {
                return false;
            }
            return true;
        }

    }
}
