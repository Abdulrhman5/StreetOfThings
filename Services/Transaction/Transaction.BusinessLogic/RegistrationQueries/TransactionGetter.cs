using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    class TransactionGetter : ITransactionGetter
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;


        public TransactionGetter(IRepository<Guid,ObjectRegistration> registrationsRepo)
        {
            _registrationsRepo = registrationsRepo;
        }
        
        public async Task<List<TransactionDto>> GetUserTransactions(string userId)
        {
            if (userId.IsNullOrEmpty())
            {
                return new List<TransactionDto>();
            }

            var trans = from rg in _registrationsRepo.Table
                        where rg.RecipientLogin.User.OriginalUserId == userId || rg.Object.OwnerUser.OriginalUserId == userId
                        let isReceived = rg.ObjectReceiving is object
                        let isReturned = rg.ObjectReceiving is object && rg.ObjectReceiving.ObjectReturning is object
                        select new TransactionDto
                        {
                            RegistrationId = rg.ObjectRegistrationId,
                            ObjectId = rg.Object.OriginalObjectId,
                            OwnerId = rg.Object.OwnerUser.OriginalUserId,
                            ReceiverId = rg.RecipientLogin.User.OriginalUserId,
                            ReceivingId = rg.ObjectReceivingId,
                            ReturnId = isReturned ? rg.ObjectReceiving.ObjectReturning.ObjectReturningId : (Guid?)null,
                            RegistredAtUtc = rg.RegisteredAtUtc,
                            ReceivedAtUtc = isReceived ? rg.ObjectReceiving.ReceivedAtUtc : (DateTime?)null,
                            ReturenedAtUtc = isReturned ? rg.ObjectReceiving.ObjectReturning.ReturnedAtUtc : (DateTime?)null,
                            TranscationType = !rg.Object.ShouldReturn ? TransactionType.Free : rg.Object.HourlyCharge.HasValue ? TransactionType.Renting : TransactionType.Lending,
                            HourlyCharge = rg.Object.HourlyCharge,
                            ShouldReturnAfter = rg.ShouldReturnItAfter,
                            TransactionStatus = rg.Status == ObjectRegistrationStatus.Canceled ? TransactionStatus.Canceled : isReturned ? TransactionStatus.Returned : isReceived ? TransactionStatus.Received : TransactionStatus.RegisteredOnly
                        };
            return await trans.ToListAsync();
        }

        public async Task<AllTransactionsListDto> GetAllTransactions()
        {
            var trans = from rg in _registrationsRepo.Table
                        let isReceived = rg.ObjectReceiving is object
                        let isReturned = rg.ObjectReceiving is object && rg.ObjectReceiving.ObjectReturning is object
                        select new TransactionDto
                        {
                            RegistrationId = rg.ObjectRegistrationId,
                            ObjectId = rg.Object.OriginalObjectId,
                            OwnerId = rg.Object.OwnerUser.OriginalUserId,
                            ReceiverId = rg.RecipientLogin.User.OriginalUserId,
                            ReceivingId = rg.ObjectReceivingId,
                            ReturnId = isReturned ? rg.ObjectReceiving.ObjectReturning.ObjectReturningId : (Guid?)null,
                            RegistredAtUtc = rg.RegisteredAtUtc,
                            ReceivedAtUtc = isReceived ? rg.ObjectReceiving.ReceivedAtUtc : (DateTime?)null,
                            ReturenedAtUtc = isReturned ? rg.ObjectReceiving.ObjectReturning.ReturnedAtUtc : (DateTime?)null,
                            TranscationType = !rg.Object.ShouldReturn ? TransactionType.Free : rg.Object.HourlyCharge.HasValue ? TransactionType.Renting : TransactionType.Lending,
                            HourlyCharge = rg.Object.HourlyCharge,
                            ShouldReturnAfter = rg.ShouldReturnItAfter,
                            TransactionStatus = rg.Status == ObjectRegistrationStatus.Canceled ? TransactionStatus.Canceled : isReturned ? TransactionStatus.Returned : isReceived ? TransactionStatus.Received : TransactionStatus.RegisteredOnly
                        };

            var transactions = await trans.ToListAsync();

            return new AllTransactionsListDto
            {
                Transactions = transactions,
                ReturnedTransactionsCount = transactions.Count(t => t.ReturnId is object),
                DeliveredTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is object),
                ReservedTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is null)
            };
        }
    }
}
