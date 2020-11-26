using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLibrary;
using EventBus;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transaction.Service.Dtos;
using Transaction.Service.Events;
using Transaction.Service.Infrastructure;
using Transaction.Service.Models;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReturnController : MyControllerBase
    {
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private ITransactionTokenManager _tokenManager;

        private OwnershipAuthorization<Guid, ObjectRegistration> _authorizer;

        private IRepository<Guid, ObjectReturning> _returningRepo;

        private IRepository<Guid, TransactionToken> _tokensRepo;

        private OwnershipAuthorization<Guid, TransactionToken> _transactionOwnershipAuthorizer;

        private UserDataManager userDataManager;

        private IEventBus _eventBus;


        [Route("generate/Token")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerateReturnToken([FromBody]GenerateReturnTokenDto generateReturnTokenDto)
        {
            if (generateReturnTokenDto is null || generateReturnTokenDto.RegistrationId.IsNullOrEmpty())
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!Guid.TryParse(generateReturnTokenDto.RegistrationId, out var guidRegistrationId))
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NULL",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
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
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.INVALID.ID",
                    Message = "Please provide valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
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
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (registration.ObjectReceiving is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.NOT.RECEIVED",
                    Message = "The object has not been received yet",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (registration.ObjectReceiving.ObjectReturning is object)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.TOKEN.GENERATE.RETURN.ALREADY.RETURNED",
                    Message = "The object has been returned",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var token = await _tokenManager.GenerateToken(registration.ObjectReceiving.ObjectReceivingId, TokenType.Returning);
            return Ok(new GenerateReturnTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                UseBeforeUtc = token.UseBeforeUtc,
                ReturnToken = token.Token
            });
        }


        [Route("return")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReturnObject([FromBody] AddObjectReturningDto returnDto)
        {
            if (returnDto == null || returnDto.ReturningToken.IsNullOrEmpty())
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var authResult = _transactionOwnershipAuthorizer.IsAuthorized(tt => tt.Type == TokenType.Returning && tt.Token == returnDto.ReturningToken,
                tt => tt.Receiving.ObjectRegistration.Object.OwnerUser);
            if (!authResult)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var theToken = (from t in _tokensRepo.Table
                            where t.Token == returnDto.ReturningToken && t.Type == TokenType.Returning
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var theReceiving = (from rc in _receivingsRepo.Table
                                where rc.Tokens.Any(returningToken => returningToken.Token == returnDto.ReturningToken)
                                select rc).Include(rc => rc.ObjectRegistration).FirstOrDefault();

            if (theReceiving is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (theReceiving.ObjectReturning is object)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            theToken.Status = TokenStatus.Used;
            var returning = new ObjectReturning
            {
                ReturnedAtUtc = DateTime.UtcNow,
                LoanerLoginId = login.LoginId,
                LoaneeLoginId = theToken.IssuerLoginId,
                ObjectReceivingId = theReceiving.ObjectReceivingId,
                ObjectReturningId = Guid.NewGuid()
            };

            _returningRepo.Add(returning);

            // This will save theToken.Status also
            await _returningRepo.SaveChangesAsync();

            var returnedAfter = DateTime.UtcNow - theReceiving.ReceivedAtUtc;

            var evnt = new TransactionReturnedIntegrationEvent()
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                RegistrationId = theReceiving.ObjectRegistrationId,
                ReturnedAtUtc = DateTime.UtcNow,
                ReturnIdId = returning.ObjectReturningId,
            };

            _eventBus.Publish(evnt);

            TimeSpan late = new TimeSpan();
            if (theReceiving.ObjectRegistration.ShouldReturnItAfter.HasValue)
            {
                late = DateTime.UtcNow - theReceiving.ReceivedAtUtc.Add(theReceiving.ObjectRegistration.ShouldReturnItAfter.Value);

                // if the value is nigative (not late)
                if (late.TotalSeconds < 0)
                {
                    late = new TimeSpan(0);
                }
            }
            var charge = theReceiving.HourlyCharge is null ? null : (float?)(theReceiving.HourlyCharge * returnedAfter.TotalHours);
            DateTime? shouldBeReturnedAtUtc = null;
            if (theReceiving.ObjectRegistration.ShouldReturnItAfter.HasValue)
                shouldBeReturnedAtUtc = theReceiving.ReceivedAtUtc.Add(theReceiving.ObjectRegistration.ShouldReturnItAfter.Value);

            return Ok(new AddObjectReturningResultDto
            {
                RegistrationId = theReceiving.ObjectRegistrationId,
                ReceivingId = theReceiving.ObjectReceivingId,
                ReturningId = returning.ObjectReturningId,
                Late = late,
                ReturnedAfter = returnedAfter,
                ShouldPay = charge,
                RegisteredAtUtc = theReceiving.ObjectRegistration.RegisteredAtUtc,
                ReceivedAtUtc = theReceiving.ReceivedAtUtc,
                ReturnedAtUtc = returning.ReturnedAtUtc,
                ShouldBeReturnedAtUtc = shouldBeReturnedAtUtc
            });
        }

    }
}
