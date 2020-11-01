using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transaction.Models;
using Transaction.Service.Dtos;
using Transaction.Service.Infrastructure;
using TransactionType = Transaction.Service.Dtos.TransactionType;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private CurrentUserCredentialsGetter _currentUserGetter;

        private TransactionStatisticsGetter _statsGetter;
        public TransactionController(TransactionStatisticsGetter statsGetter)
        {
            _statsGetter = statsGetter;
        }


        [Route("foruser")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TransactionsForUser(string userId)
        {
            if (userId.IsNullOrEmpty())
            {
                return Ok(new List<RegistrationDto>());
            }

            var trans = from rg in _registrationsRepo.Table
                        where rg.RecipientLogin.UserId == Guid.Parse(userId) || rg.Object.OwnerUserId == Guid.Parse(userId)
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
                            TransactionStatus.Received : TransactionStatus.RegisteredOnly
                        };
            return Ok(await trans.ToListAsync());
        }

        [Route("allTranses")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllTransactions()
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

            var transactions = await trans.ToListAsync();

            return Ok(new AllTransactionsListDto
            {
                Transactions = transactions,
                ReturnedTransactionsCount = transactions.Count(t => t.ReturnId is object),
                DeliveredTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is object),
                ReservedTransactionsCount = transactions.Count(t => t.ReturnId is null && t.ReceivingId is null)
            });
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

        [Route("stats/today")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsToday()
        {
            var stats = await _statsGetter.GetTransactionsCountOverToday();
            return Ok(stats);
        }

        [Route("stats/lastMonth")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsOverMonth()
        {
            var stats = await _statsGetter.GetTransactionsCountOverMonth();
            return Ok(stats);
        }

        [Route("stats/lastYear")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsYear()
        {
            var stats = await _statsGetter.GetTransactionsCountOverYear();
            return Ok(stats);
        }
    }
}
