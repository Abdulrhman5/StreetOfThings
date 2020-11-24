using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using System.Linq;
using Transaction.Core.Extensions;

namespace Transaction.Core.Queries
{
    class AllRegistrationQueryHandler : IRequestHandler<AllRegistrationQuery, AllTransactionsListDto>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IQueryableHelper _queryHelper;

        public async Task<AllTransactionsListDto> Handle(AllRegistrationQuery request, CancellationToken cancellationToken)
        {
            var trans = from rg in _registrationsRepo.Table
                        let isReceived = rg.ObjectReceiving != null
                        let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
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
                            TransactionStatus = rg.Status == ObjectRegistrationStatus.Canceled ?
                            TransactionStatus.Canceled : isReturned ?
                            TransactionStatus.Returned : isReceived ?
                            TransactionStatus.Received : TransactionStatus.RegisteredOnly,
                            ReturnStatus = GetReturnStatus(rg)
                        };

            var transactions = await trans.ToListAsync(_queryHelper);

            return new AllTransactionsListDto
            {
                Transactions = transactions,
                ReturnedTransactionsCount = transactions.Count(t => t.ReturnId is object),
                DeliveredTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is object),
                ReservedTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is null)
            };
        }

        private static ReturnStatus GetReturnStatus(ObjectRegistration registration)
        {
            if (registration.Status == ObjectRegistrationStatus.Canceled)
            {
                return ReturnStatus.NotTakenYet;
            }

            if (registration.ObjectReceiving is null)
            {
                return ReturnStatus.NotTakenYet;
            }

            if (registration.ObjectReceiving.ObjectReturning is object)
            {
                return ReturnStatus.Returned;
            }

            // it is free object
            if (!registration.ShouldReturnItAfter.HasValue)
            {
                return ReturnStatus.NotDueYet;
            }

            if (registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) <= DateTime.UtcNow)
            {
                return ReturnStatus.NotDueYet;
            }

            if (registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) > DateTime.UtcNow.AddHours(24))
            {
                return ReturnStatus.PossibleTheft;
            }

            if (registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) > DateTime.UtcNow)
            {
                return ReturnStatus.Delayed;
            }

            return ReturnStatus.Returned;
        }
    }
}
