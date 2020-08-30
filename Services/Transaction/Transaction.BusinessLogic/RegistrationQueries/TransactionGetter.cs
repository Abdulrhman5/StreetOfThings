using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.RegistrationQueries
{
    class TransactionGetter : ITransactionGetter
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private CurrentUserCredentialsGetter _currentUserGetter;


        public TransactionGetter(IRepository<Guid, ObjectRegistration> registrationsRepo, CurrentUserCredentialsGetter userGetter)
        {
            _registrationsRepo = registrationsRepo;
            _currentUserGetter = userGetter;
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

        /// <summary>
        /// Get the transactions of The current user 
        /// Where the current user is a owner 
        /// </summary>
        /// <param name="pagingArguments"></param>
        /// <returns></returns>
        public async Task<List<TransactionDto>> GetTransactionsWhereUserIsOwner(PagingArguments pagingArguments)
        {
            var currentUser = _currentUserGetter.GetCuurentUser();
            if (currentUser?.UserId is null)
            {
                throw new UnauthorizedAccessException();
            }
            var userId = currentUser.UserId;

            var trans = from rg in _registrationsRepo.Table
                        where rg.Object.OwnerUser.OriginalUserId == userId
                        let isReceived = rg.ObjectReceiving is object
                        let isReturned = rg.ObjectReceiving is object && rg.ObjectReceiving.ObjectReturning is object
                        orderby rg.RegisteredAtUtc descending
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
            return await trans.SkipTakeAsync(pagingArguments);
        }

        /// <summary>
        /// Get the transactions of The current user 
        /// Where the current user is a recipient 
        /// </summary>
        /// <param name="pagingArguments"></param>
        /// <returns></returns>
        public async Task<List<TransactionDto>> GetTransactionsWhereUserIsRecipient(PagingArguments pagingArguments)
        {
            var currentUser = _currentUserGetter.GetCuurentUser();
            if (currentUser?.UserId is null)
            {
                throw new UnauthorizedAccessException();
            }
            var userId = currentUser.UserId;

            var trans = from rg in _registrationsRepo.Table
                        where rg.RecipientLogin.User.OriginalUserId == userId
                        let isReceived = rg.ObjectReceiving is object
                        let isReturned = rg.ObjectReceiving is object && rg.ObjectReceiving.ObjectReturning is object
                        orderby rg.RegisteredAtUtc descending
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
            return await trans.SkipTakeAsync(pagingArguments);
        }
    }
}
