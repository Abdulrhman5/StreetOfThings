using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.ReturningCommands
{
    public class ReturnTokenGenerator
    {
        private IRepository<int, OfferedObject> _objectsRepo;

        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private ITransactionTokenManager _tokenManager;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private OwnershipAuthorization<Guid, ObjectRegistration> _authorizer;

        public ReturnTokenGenerator(IRepository<int, OfferedObject> objectsRepo, IRepository<Guid, ObjectReceiving> receivingsRepo, IRepository<Guid, ObjectRegistration> registrationsRepo, ITransactionTokenManager tokenManager, CurrentUserCredentialsGetter credentialsGetter, OwnershipAuthorization<Guid, ObjectRegistration> authorizer)
        {
            _objectsRepo = objectsRepo;
            _receivingsRepo = receivingsRepo;
            _registrationsRepo = registrationsRepo;
            _tokenManager = tokenManager;
            _credentialsGetter = credentialsGetter;
            _authorizer = authorizer;
        }

        public async Task<CommandResult<GenerateReturnTokenResultDto>> GenerateToken(GenerateReturnTokenDto generateReturnTokenDto)
        {
            if (generateReturnTokenDto is null || generateReturnTokenDto.RegistrationId.IsNullOrEmpty())
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            if (!Guid.TryParse(generateReturnTokenDto.RegistrationId, out var guidRegistrationId))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.INVALID.ID",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            var registration = (from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == guidRegistrationId
                                select r)
                                .Include(r => r.ObjectReceiving)
                                .ThenInclude(r => r.ObjectReturning)
                                .Include(r => r.Object)
                                .FirstOrDefault();

            if (registration is null || registration.Status == ObjectRegistrationStatus.Canceled)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.INVALID.ID",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            //if (!registration.Object.ShouldReturn)
            //{
            //    return new ErrorMessage
            //    {
            //        ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.FREE.OBJECT",
            //        Message = "The Object now is yours, you don't have to return it",
            //        StatusCode = System.Net.HttpStatusCode.BadRequest
            //    }.ToCommand<GenerateReturnTokenResultDto>();
            //}

            if (!_authorizer.IsAuthorized(or => or.ObjectRegistrationId == guidRegistrationId, or => or.RecipientLogin.User))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            if (registration.ObjectReceiving is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NOT.RECEIVED",
                    Message = "The object has not been received yet",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            if (registration.ObjectReceiving.ObjectReturning is object)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.ALREADY.RETURNED",
                    Message = "The object has been returned",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<GenerateReturnTokenResultDto>();
            }

            var token = await _tokenManager.GenerateToken(registration.ObjectReceiving.ObjectReceivingId, TokenType.Returning);
            return new CommandResult<GenerateReturnTokenResultDto>(new GenerateReturnTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                UseBeforeUtc = token.UseBeforeUtc,
                ReturnToken = token.Token
            });
        }
    }
}
