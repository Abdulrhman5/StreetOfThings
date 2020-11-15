using FluentValidation;
using System;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;

namespace Transaction.Core.Validations
{
    public class CreateRegistrationValidator : AbstractValidator<CreateRegistrationCommand>
    {
        private IObjectDataManager _objectGetter;
        private IRepository<int, OfferedObject> _objectsRepo;
        private IRepository<Guid, ObjectReceiving> _objectReceiving;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IUserDataManager _userDataManager;


        public CreateRegistrationValidator(IObjectDataManager objectGetter,
            IRepository<int, OfferedObject> objectsRepo,
            IRepository<Guid, ObjectReceiving> objectReceiving,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IUserDataManager userDataManager)
        {
            _objectGetter = objectGetter;
            _objectsRepo = objectsRepo;
            _objectReceiving = objectReceiving;
            _registrationsRepo = registrationsRepo;
            _userDataManager = userDataManager;


            CascadeMode = CascadeMode.Stop;
            RuleFor(reg => reg)
                .NotNull()
                .WithMessage("TRANSACTION.OBJECT.RESERVE.NULL");

            RuleFor(reg => reg)
                .MustAsync(async (reg, ct) =>
                {
                    var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
                    return login != null;
                })
                .WithMessage("TRANSACTION.OBJECT.RESERVE.NOT.AUTHORIZED");

            RuleFor(reg => reg.ObjectId)
                .MustAsync(async (objectId, ct) => await _objectGetter.GetObjectAsync(objectId) is object)
                .WithMessage("TRANSACTION.OBJECT.RESERVE.NOT.EXISTS");

            RuleFor(reg => reg)
                .Must(IsObjectAvailable)
                .WithMessage("TRANSACTION.OBJECT.RESERVE.NOT.AVAILABLE");

            RuleFor(registration => registration.ShouldReturnAfter)
                .NotNull()
                .When(reg =>
                {
                    return _objectsRepo.Get(reg.ObjectId).ShouldReturn;
                })
                .WithMessage("TRANSACTION.OBJECT.RESERVE.SHOULDRETURN.NULL");
        }

        private bool IsObjectAvailable(CreateRegistrationCommand reg)
        {
            var receivings = from receiving in _objectReceiving.Table
                             where receiving.ObjectRegistration.ObjectId == reg.ObjectId
                             select receiving;

            // If The object has receivings and at least one of them has no returning
            // then the object is not available
            if (receivings.Any(r => r.ObjectReturning == null))
            {
                return false;
            }

            var existingRegistrations = from registration in _registrationsRepo.Table
                                        where registration.ObjectId == reg.ObjectId
                                        select registration;

            // if the object has previous registration
            // if any one of them has no receiving; meaning there is some user registered for this object but did not take the object yet
            // OR if the registration has a receiving but has no returning; meaning the object has not been returned yet
            if (existingRegistrations.Any(reg => reg.ObjectReceiving == null || reg.ObjectReceiving.ObjectReturning == null))
            {
                return false;
            }

            // the object is available
            return true;
        }
    }
}
