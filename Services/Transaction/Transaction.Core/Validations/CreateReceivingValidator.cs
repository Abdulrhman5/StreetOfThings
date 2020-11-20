using FluentValidation;
using System;
using System.Threading.Tasks;
using Transaction.Core.Commands;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;
namespace Transaction.Core.Validations
{
    class CreateReceivingValidator : AbstractValidator<CreateReceivingCommand>
    {
        private IUserDataManager _userDataManager;
        private IOwnershipAuthorization<Guid, TransactionToken> _ownershipAuthorizer;
        private IRepository<Guid, TransactionToken> _tokensRepo;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        public CreateReceivingValidator()
        {
            RuleFor(receiving => receiving)
                .NotNull()
                .WithMessage("TRANSACTION.RECEIVING.ADD.NULL");

            RuleFor(receiving => receiving.RegistrationToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("TRANSACTION.RECEIVING.ADD.NULL");

            RuleFor(receivibg => receivibg)
                .MustBeAuthorized(_userDataManager)
                .WithMessage("TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED");

            RuleFor(receiving => receiving)
                .MustBeAuthorized(_ownershipAuthorizer,
                    identifingEntityExpression: receiving => tt => tt.Type == TokenType.Receiving && tt.Token == receiving.RegistrationToken,
                    navigationToUser: receiving => tt => tt.Registration.Object.OwnerUser)
                .WithMessage("TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED");

            RuleFor(receiving => receiving.RegistrationToken)
                .Must(DoesTokenExistsAndAvailable)
                .WithMessage("TRANSACTION.RECEIVING.ADD.NOT.EXISTS");

            RuleFor(receiving => receiving.RegistrationToken)
                .Must(DoesRegistrationExistsAndNotTaken)
                .WithMessage("TRANSACTION.RECEIVING.ADD.NOT.EXISTS");

            RuleFor(receiving => receiving.RegistrationToken)
                .Must(DoesObjectIsAtOwner)
                .WithMessage("TRANSACTION.RECEIVING.ADD.OBJECT.TAKEN");


        }

        private bool DoesTokenExistsAndAvailable(string registrationToken) 
        {
            var theToken = (from t in _tokensRepo.Table
                            where t.Token == registrationToken && t.Type == TokenType.Receiving
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return false;
            }

            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return false;
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return false;
            }
            return true;
        } 

        private bool DoesRegistrationExistsAndNotTaken(string registrationToken)
        {
            var theRegistration = (from rg in _registrationsRepo.Table
                                   where rg.Tokens.Any(rt => rt.Token == registrationToken)
                                   select rg).FirstOrDefault();

            if (theRegistration is null)
            {
                return false;    
            }


            if (theRegistration.ObjectReceiving is object)
            {
                return false;
            }
            return true;
        }

        private bool DoesObjectIsAtOwner(string registrationToken)
        {

            var objectRegistrations = from rg in _registrationsRepo.Table
                                      where rg.ObjectId == (from regstration in _registrationsRepo.Table
                                                            where regstration.Tokens.Any(rt => rt.Token == registrationToken)
                                                            select regstration).FirstOrDefault().ObjectId
                                      select rg;

            // if not all of them returned or not received
            if (!objectRegistrations.All(or => or.ObjectReceiving == null || or.ObjectReceiving.ObjectReturning != null))
            {
                return false;
            }
            return true;
        }
    }
}
