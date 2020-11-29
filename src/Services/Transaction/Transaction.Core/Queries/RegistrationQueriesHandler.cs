using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Extensions;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;

namespace Transaction.Core.Queries
{
    class RegistrationQueriesHandler : IRequestHandler<RegisterationForUserQuery, List<RegistrationDto>>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IUserDataManager _userDataManager;
        private IQueryableHelper _queryHelper;

        public RegistrationQueriesHandler(IRepository<Guid, ObjectRegistration> registrationsRepo,
            IUserDataManager userDataManager, IQueryableHelper queryHelper)
        {
            _registrationsRepo = registrationsRepo;
            _userDataManager = userDataManager;
            _queryHelper = queryHelper;
        }

        public async Task<List<RegistrationDto>> Handle(RegisterationForUserQuery request, CancellationToken cancellationToken)
        {
            var user = _userDataManager.GetCurrentUser();
            if (user.UserId is null)
            {
                throw new UnauthorizedAccessException();
            }
            var userId = user.UserId;

            Expression<Func<ObjectRegistration, bool>> typeFilter = null;
            if (request.RegistrationType == RegistrationForUserType.UserAsOwner)
            {
                typeFilter = rg => rg.Object.OwnerUser.UserId == Guid.Parse(userId);
            }
            else if (request.RegistrationType == RegistrationForUserType.UserAsRecipient)
            {
                typeFilter = rg => rg.RecipientLogin.UserId == Guid.Parse(userId);
            }
            else if(request.RegistrationType == RegistrationForUserType.All)
            {
                typeFilter = rg => rg.RecipientLogin.UserId == Guid.Parse(userId) || rg.Object.OwnerUserId == Guid.Parse(userId);
            }

            var registrations = from rg in _registrationsRepo.Table
                                          .Where(typeFilter)
                                let isReceived = rg.ObjectReceiving != null
                                let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
                                orderby rg.RegisteredAtUtc descending
                                select new RegistrationDto
                                {
                                    RegistrationId = rg.ObjectRegistrationId,
                                    ObjectId = rg.Object.OriginalObjectId,
                                    OwnerId = rg.Object.OwnerUserId.ToString(),
                                    ReceiverId = rg.RecipientLogin.UserId.ToString(),
                                    ReceivingId = rg.ObjectReceiving.ObjectReceivingId,
                                    ReturnId = isReturned ? rg.ObjectReceiving.ObjectReturning.ObjectReturningId : (Guid?)null,
                                    RegistredAtUtc = rg.RegisteredAtUtc,
                                    ReceivedAtUtc = isReceived ? rg.ObjectReceiving.ReceivedAtUtc : (DateTime?)null,
                                    ReturenedAtUtc = isReturned ? rg.ObjectReceiving.ObjectReturning.ReturnedAtUtc : (DateTime?)null,
                                    TranscationType = !rg.Object.ShouldReturn ? TransactionType.Free : rg.Object.HourlyCharge.HasValue ? TransactionType.Renting : TransactionType.Lending,
                                    HourlyCharge = rg.Object.HourlyCharge,
                                    ShouldReturnAfter = rg.ShouldReturnItAfter,
                                    TransactionStatus = rg.Status == ObjectRegistrationStatus.Canceled ? TransactionStatus.Canceled :
                                        isReturned ? TransactionStatus.Returned :
                                        isReceived ? TransactionStatus.Received :
                                        TransactionStatus.RegisteredOnly,
                                };

            return await registrations.SkipTakeAsync(_queryHelper, request);
        }
    }
}
