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

namespace Transaction.Core.Commands
{
    class RefreshRegistrationTokenCommandHandler : IRequestHandler<RefreshRegistrationTokenCommand, RegistrationTokenResultDto>
    {
        private ITransactionTokenManager _tokenManager;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IUserDataManager _userDataManager;
        public async Task<RegistrationTokenResultDto> Handle(RefreshRegistrationTokenCommand request, CancellationToken cancellationToken)
        {
            var user = _userDataManager.GetCurrentUser();

            var registrations = from r in _registrationsRepo.Table
                                where r.Object.OriginalObjectId == request.ObjectId && r.RecipientLogin.UserId == Guid.Parse(user.UserId) &&
                                r.Status == ObjectRegistrationStatus.OK && r.ExpiresAtUtc > DateTime.UtcNow && r.ObjectReceiving == null
                                select r;


            var token = await _tokenManager.GenerateToken(registrations.FirstOrDefault().ObjectRegistrationId, TokenType.Receiving);

            return new RegistrationTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                RegistrationToken = token.Token,
                UseBeforeUtc = token.UseBeforeUtc
            };
        }
    }
}
