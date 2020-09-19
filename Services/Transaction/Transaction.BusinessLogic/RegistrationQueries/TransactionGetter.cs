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
                        where rg.RecipientLogin.UserId == Guid.Parse(userId) || rg.Object.OwnerUserId == Guid.Parse(userId)
                        let isReceived = rg.ObjectReceiving != null
                        let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
                        select new TransactionDto
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
                            TransactionStatus.Received : TransactionStatus.RegisteredOnly
                        };
            return await trans.ToListAsync();
        }

        public async Task<AllTransactionsListDto> GetAllTransactions()
        {
            var trans = from rg in _registrationsRepo.Table
                        let isReceived = rg.ObjectReceiving != null
                        let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
                        select new TransactionDto
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
                        where rg.Object.OwnerUser.UserId == Guid.Parse(userId)
                        let isReceived = rg.ObjectReceiving != null
                        let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
                        orderby rg.RegisteredAtUtc descending
                        select new TransactionDto
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
                            TransactionStatus.Received : TransactionStatus.RegisteredOnly
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
                        where rg.RecipientLogin.UserId == Guid.Parse(userId)
                        let isReceived = rg.ObjectReceiving != null
                        let isReturned = rg.ObjectReceiving != null && rg.ObjectReceiving.ObjectReturning != null
                        orderby rg.RegisteredAtUtc descending
                        select new TransactionDto
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
                            TransactionStatus = rg.Status == ObjectRegistrationStatus.Canceled ? TransactionStatus.Canceled:
                                isReturned ? TransactionStatus.Returned :
                                isReceived ? TransactionStatus.Received :
                                TransactionStatus.RegisteredOnly,
                        };


            return await trans.SkipTakeAsync(pagingArguments);
        }

        public static ReturnStatus GetReturnStatus(ObjectRegistration registration)
        {
            if (registration.Status == ObjectRegistrationStatus.Canceled)
            {
                return ReturnStatus.NotTakenYet;
            }

            if (registration.ObjectReceiving is null)
            {
                return ReturnStatus.NotTakenYet;
            }

            if(registration.ObjectReceiving.ObjectReturning is object)
            {
                return ReturnStatus.Returned;
            }

            // it is free object
            if (!registration.ShouldReturnItAfter.HasValue)
            {
                return ReturnStatus.NotDueYet;
            }

            if(registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) <= DateTime.UtcNow)
            {
                return ReturnStatus.NotDueYet;
            } 
            
            if(registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) > DateTime.UtcNow.AddHours(24))
            {
                return ReturnStatus.PossibleTheft;
            }    
            
            if(registration.ObjectReceiving.ReceivedAtUtc.Add(registration.ShouldReturnItAfter.Value) > DateTime.UtcNow)
            {
                return ReturnStatus.Delayed;
            }

            return ReturnStatus.Returned;
        }
    }
}
