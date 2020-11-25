using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Service.Dtos;
using System.Linq;
using Transaction.Core.Extensions;

namespace Transaction.Core.Commands
{
    class RefreshRegistrationTokenCommandHandler : IRequestHandler<RefreshRegistrationTokenCommand, CommandResult<RegistrationTokenResultDto>>
    {
        private ITransactionTokenManager _tokenManager;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IUserDataManager _userDataManager;

        public RefreshRegistrationTokenCommandHandler(ITransactionTokenManager tokenManager,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IUserDataManager userDataManager)
        {
            _tokenManager = tokenManager;
            _registrationsRepo = registrationsRepo;
            _userDataManager = userDataManager;
        }

        public async Task<CommandResult<RegistrationTokenResultDto>> Handle(RefreshRegistrationTokenCommand request, CancellationToken cancellationToken)
        {
            var user = _userDataManager.GetCurrentUser();
            if (user == null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.USER.UNKOWN",
                    Message = "Please login",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<RegistrationTokenResultDto>();
            }
            var registrations = from r in _registrationsRepo.Table
                                where r.Object.OriginalObjectId == request.ObjectId && r.RecipientLogin.UserId == Guid.Parse(user.UserId) &&
                                r.Status == ObjectRegistrationStatus.OK && r.ExpiresAtUtc > DateTime.UtcNow && r.ObjectReceiving == null
                                select r;

            if (!registrations.Any())
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.NOT.VALID",
                    Message = "You have no valid registration",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<RegistrationTokenResultDto>();
            }



            var token = await _tokenManager.GenerateToken(registrations.FirstOrDefault().ObjectRegistrationId, TokenType.Receiving);

            return new CommandResult<RegistrationTokenResultDto>(new RegistrationTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                RegistrationToken = token.Token,
                UseBeforeUtc = token.UseBeforeUtc
            });
        }
    }
}
