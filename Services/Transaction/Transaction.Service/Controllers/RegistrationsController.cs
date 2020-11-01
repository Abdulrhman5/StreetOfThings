using CommonLibrary;
using EventBus;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.Service.Models;
using Transaction.Service.Events;
using Transaction.Service.Infrastructure;
using Transaction.Service.Dtos;
using TransactionType = Transaction.Service.Dtos.TransactionType;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationsController : MyControllerBase
    {

        private readonly IUserDataManager _userDataManager;

        private readonly IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private readonly ObjectDataManager _objectDataManager;

        private readonly IRepository<Guid, ObjectReceiving> _objectReceiving;

        private IEventBus _eventBus;

        private readonly ITransactionTokenManager _tokenManager;

        private int maximumHoursForFreeLending = 6;

        private int maximumHoursForReservationExpiration = 24;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private OwnershipAuthorization<Guid, ObjectRegistration> _ownershipAuth;

        public RegistrationsController(
            IUserDataManager userDataManager,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            ObjectDataManager objectDataManager,
            IRepository<Guid, ObjectReceiving> objectReceiving,
            IEventBus eventBus,
            ITransactionTokenManager tokenManager,
            int maximumHoursForFreeLending,
            int maximumHoursForReservationExpiration,
            CurrentUserCredentialsGetter credentialsGetter,
            OwnershipAuthorization<Guid, ObjectRegistration> ownershipAuth)
        {
            _userDataManager = userDataManager;
            _registrationsRepo = registrationsRepo;
            _objectDataManager = objectDataManager;
            _objectReceiving = objectReceiving;
            _eventBus = eventBus;
            _tokenManager = tokenManager;
            this.maximumHoursForFreeLending = maximumHoursForFreeLending;
            this.maximumHoursForReservationExpiration = maximumHoursForReservationExpiration;
            _credentialsGetter = credentialsGetter;
            _ownershipAuth = ownershipAuth;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddNewRegistrationDto newRegistrationDto)
        {
            ErrorMessage ObjectNotAvailable = new ErrorMessage
            {
                ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AVAILABLE",
                Message = "This object is not available",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };

            var user = await _userDataManager.AddCurrentUserIfNeeded();
            if (user.Login == null)
            {
                return StatusCode(new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this operation",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            if (newRegistrationDto is null)
            {
                return StatusCode(new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NULL",
                    Message = "Please send a valid information",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var @object = await _objectDataManager.GetObjectAsync(newRegistrationDto.ObjectId);
            if (@object is null)
            {
                return StatusCode(new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.EXISTS",
                    Message = "The object specified does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            // Should Not Return and is not taken right now
            if (!@object.ShouldReturn)
            {
                var receivings = from receiving in _objectReceiving.Table
                                 where receiving.ObjectRegistration.ObjectId == @object.OfferedObjectId
                                 select receiving;

                // If The object has receiving and all of them has returnings 
                if (receivings.Any(r => r.ObjectReturning == null))
                {
                    return StatusCode(ObjectNotAvailable);
                }
            }

            // See Previous registrations

            var existingRegistrations = from registration in _registrationsRepo.Table
                                        where registration.RecipientLogin.UserId == user.User.UserId && registration.ObjectId == @object.OfferedObjectId
                                        select registration;

            // If The user taken and has this object OR If the user has another registeration pending receiving
            if (existingRegistrations.Any(reg => reg.ObjectReceiving == null || reg.ObjectReceiving.ObjectReturning == null))
            {
                return StatusCode(ObjectNotAvailable);
            }

            TimeSpan? shouldReturnItAfter;
            if (@object.ShouldReturn)
            {
                // If the object should return but the user has not specified the time he should return the object
                if (!newRegistrationDto.ShouldReturnAfter.HasValue)
                {
                    return StatusCode(new ErrorMessage
                    {
                        ErrorCode = "TRANSACTION.OBJECT.RESERVE.SHOULDRETURN.NULL",
                        Message = "Please specify when you will return this object",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }

                if (@object.HourlyCharge.HasValue)
                {
                    shouldReturnItAfter = new TimeSpan(newRegistrationDto.ShouldReturnAfter.Value, 0, 0);
                }
                else
                {
                    if (newRegistrationDto.ShouldReturnAfter > maximumHoursForFreeLending)
                        shouldReturnItAfter = new TimeSpan(maximumHoursForFreeLending, 0, 0);
                    else
                        shouldReturnItAfter = new TimeSpan(newRegistrationDto.ShouldReturnAfter.Value, 0, 0);
                }
            }
            else
            {
                shouldReturnItAfter = null;
            }


            var registrationModel = new ObjectRegistration
            {
                ObjectRegistrationId = Guid.NewGuid(),
                RegisteredAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(maximumHoursForReservationExpiration),
                ObjectId = @object.OfferedObjectId,
                Status = ObjectRegistrationStatus.OK,
                RecipientLoginId = user.Login.LoginId,
                ShouldReturnItAfter = shouldReturnItAfter,
            };

            _registrationsRepo.Add(registrationModel);
            await _registrationsRepo.SaveChangesAsync();

            var integrationEvent = new NewRegistrationIntegrationEvent()
            {
                Id = Guid.NewGuid(),
                OccuredAt = registrationModel.RegisteredAtUtc,
                ObjectId = @object.OriginalObjectId,
                RecipiantId = user.User.UserId.ToString(),
                ShouldReturn = @object.ShouldReturn,
                RegisteredAt = registrationModel.RegisteredAtUtc,
                RegistrationId = registrationModel.ObjectRegistrationId
            };

            // Broadcast an event;
            _eventBus.Publish(integrationEvent);


            var token = await _tokenManager.GenerateToken(registrationModel.ObjectRegistrationId, TokenType.Receiving);

            var dto = new AddNewRegistrationResultDto
            {
                ObjectId = registrationModel.Object.OriginalObjectId,
                RegistrationId = registrationModel.ObjectRegistrationId,
                ShouldBeReturnedAfterReceving = registrationModel.ShouldReturnItAfter,
                RegistrationExpiresAtUtc = registrationModel.ExpiresAtUtc,
                RegistrationToken = new RegistrationTokenResultDto
                {
                    RegistrationToken = token.Token,
                    CreatedAtUtc = token.IssuedAtUtc,
                    UseBeforeUtc = token.UseBeforeUtc
                }
            };

            return StatusCode(200, dto);
        }

        [Route("refresh")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRegistrationTokenDto tokenRefresh)
        {
            if (tokenRefresh is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var user = _credentialsGetter.GetCuurentUser();
            if (user == null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.USER.UNKOWN",
                    Message = "Please login",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }
            var registrations = from r in _registrationsRepo.Table
                                where r.Object.OriginalObjectId == tokenRefresh.ObjectId && r.RecipientLogin.UserId == Guid.Parse(user.UserId) &&
                                r.Status == ObjectRegistrationStatus.OK && r.ExpiresAtUtc > DateTime.UtcNow && r.ObjectReceiving == null
                                select r;

            if (!registrations.Any())
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.REFRESH.NOT.VALID",
                    Message = "You have no valid registration",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var token = await _tokenManager.GenerateToken(registrations.FirstOrDefault().ObjectRegistrationId, TokenType.Receiving);

            return Ok(new RegistrationTokenResultDto
            {
                CreatedAtUtc = token.IssuedAtUtc,
                RegistrationToken = token.Token,
                UseBeforeUtc = token.UseBeforeUtc
            });
        }

        [Route("cancel")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelRegistration([FromBody] CancelRegistrationDto cancelRegistration)
        {
            if (cancelRegistration == null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!Guid.TryParse(cancelRegistration.RegistrationId, out var registrationIdGuid))
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.INVALID.DATA",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var authResult = _ownershipAuth.IsAuthorized(or => or.ObjectRegistrationId == registrationIdGuid, or => or.RecipientLogin.User);
            if (!authResult)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAUTHORIZED",
                    Message = "You are not authorized to cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var registrations = from r in _registrationsRepo.Table
                                where r.ObjectRegistrationId == registrationIdGuid &&
                                r.ObjectReceiving == null &&
                                r.Status == ObjectRegistrationStatus.OK &&
                                r.ExpiresAtUtc > DateTime.UtcNow
                                select r;

            if (!registrations.Any())
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.REGISTRATION.CANCEL.UNAVAILABLE",
                    Message = "You can not cancel this registration",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }


            var theRegistration = _registrationsRepo.Get(registrationIdGuid);
            theRegistration.Status = ObjectRegistrationStatus.Canceled;
            await _registrationsRepo.SaveChangesAsync();
            _eventBus.Publish(new TransactionCancelledIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                RegistrationId = theRegistration.ObjectRegistrationId,
                CancelledAtUtc = DateTime.UtcNow,
            });
            return StatusCode(200, new
            {
                Message = "A registration has been canceled"
            });
        }

        [Route("MeAsRecipient")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersRegistrationsOnOtherUsersObjects([FromQuery] PagingArguments pagingArguments)
        {
            var currentUser = _credentialsGetter.GetCuurentUser();
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
            return Ok(await trans.SkipTakeAsync(pagingArguments));
        }

        [Route("MeAsOwner")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserObjectsTransactions([FromQuery] PagingArguments pagingArguments)
        {
            var currentUser = _credentialsGetter.GetCuurentUser();
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


            return Ok(await trans.SkipTakeAsync(pagingArguments));
        }
    }
}
