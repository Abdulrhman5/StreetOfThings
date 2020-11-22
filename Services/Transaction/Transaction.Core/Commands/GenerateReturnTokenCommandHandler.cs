using EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Exceptions;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;
using Transaction.Core.Extensions;

namespace Transaction.Core.Commands
{
    public class GenerateReturnTokenCommandHandler : IRequestHandler<GenerateReturnTokenCommand, GenerateReturnTokenResultDto>
    {

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private ITransactionTokenManager _tokenManager;
        private IOwnershipAuthorization<Guid, ObjectRegistration> _authorizer;
        private IQueryableHelper _queryHelper;

        public GenerateReturnTokenCommandHandler(IRepository<Guid, ObjectRegistration> registrationsRepo,
            ITransactionTokenManager tokenManager, 
            IOwnershipAuthorization<Guid, ObjectRegistration> authorizer,
            IQueryableHelper queryHelper)
        {
            _registrationsRepo = registrationsRepo;
            _tokenManager = tokenManager;
            _authorizer = authorizer;
            _queryHelper = queryHelper;
        }

        public async Task<GenerateReturnTokenResultDto> Handle(GenerateReturnTokenCommand request, CancellationToken cancellationToken)
        {
            var guidRegistrationId = Guid.Parse(request.RegistrationId);

            var registration = (from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == guidRegistrationId
                                select r)
                                .Include($"{nameof(ObjectRegistration.ObjectReceiving)}.{nameof(ObjectRegistration.ObjectReceiving.ObjectReturning)}, {nameof(ObjectRegistration.Object)}", _queryHelper)
                                .FirstOrDefault();

            if (registration is null || registration.Status == ObjectRegistrationStatus.Canceled)
            {
                throw new DomainException(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.INVALID.ID",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!_authorizer.IsAuthorized(or => or.ObjectRegistrationId == guidRegistrationId, or => or.RecipientLogin.User))
            {
                throw new DomainException(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            if (registration.ObjectReceiving is null)
            {
                throw new DomainException(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NOT.RECEIVED",
                    Message = "The object has not been received yet",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (registration.ObjectReceiving.ObjectReturning is object)
            {
                throw new DomainException(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.ALREADY.RETURNED",
                    Message = "The object has been returned",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            var token = await _tokenManager.GenerateToken(registration.ObjectReceiving.ObjectReceivingId, TokenType.Returning);
            return new GenerateReturnTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                UseBeforeUtc = token.UseBeforeUtc,
                ReturnToken = token.Token
            };
        }
    }
}
