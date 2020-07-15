using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class RegistrationTokenRefresher
    {
        private IRepository<int, OfferedObject> _objectsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private ITransactionTokenManager _tokenManager;

        private CurrentUserCredentialsGetter _credentialsGetter;

        public RegistrationTokenRefresher(IRepository<int, OfferedObject> objectsRepo,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            ITransactionTokenManager tokenManager,
            CurrentUserCredentialsGetter credentialsGetter)
        {
            _objectsRepo = objectsRepo;
            _tokenManager = tokenManager;
            _credentialsGetter = credentialsGetter;
            _registrationsRepo = registrationsRepo;
        }

        public async Task<CommandResult<RefreshRegistrationTokenResultDto>> RefreshToken(int objectId)
        {
            var user = _credentialsGetter.GetCuurentUser();
            if (user == null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.USER.UNKOWN",
                    Message = "Please login",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<RefreshRegistrationTokenResultDto>();
            }
            var registrations = from r in _registrationsRepo.Table
                                where r.Object.OriginalObjectId == objectId && r.RecipientLogin.User.OriginalUserId == user.UserId &&
                                r.Status == ObjectRegistrationStatus.OK && r.ExpiresAtUtc > DateTime.UtcNow && r.ObjectReceiving == null
                                select r;

            if (!registrations.Any())
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.NOT.VALID",
                    Message = "You have no valid registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<RefreshRegistrationTokenResultDto>();
            }

            var token = await _tokenManager.GenerateToken(registrations.FirstOrDefault().ObjectRegistrationId, TokenType.Receiving);

            return new CommandResult<RefreshRegistrationTokenResultDto>(new RefreshRegistrationTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                RegistrationToken = token.Token,
                UseBeforeUtc = token.UseBeforeUtc
            });
        }
    }
}
