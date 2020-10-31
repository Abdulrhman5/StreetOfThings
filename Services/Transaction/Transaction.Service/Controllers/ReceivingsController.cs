using CommonLibrary;
using EventBus;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Transaction.DataAccessLayer;
using Transaction.Models;
using Transaction.Service.Events;
using Transaction.Service.Infrastructure;
using Transaction.Service.Dtos;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReceivingsController : MyControllerBase
    {
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private IRepository<Guid, TransactionToken> _tokensRepo;

        private IRepository<int, OfferedObject> _objectRepo;

        private OwnershipAuthorization<Guid, TransactionToken> _ownershipAuthorization;

        private IUserDataManager userDataManager;

        private IEventBus _eventBus;

        public ReceivingsController(IRepository<Guid, ObjectReceiving> receivingsRepo,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IRepository<Guid, TransactionToken> tokensRepo,
            OwnershipAuthorization<Guid, TransactionToken> ownershipAuthorization,
            IUserDataManager userDataManager,
            IEventBus eventBus, 
            IRepository<int, OfferedObject> objectRepo)
        {
            _receivingsRepo = receivingsRepo;
            _registrationsRepo = registrationsRepo;
            _tokensRepo = tokensRepo;
            _ownershipAuthorization = ownershipAuthorization;
            this.userDataManager = userDataManager;
            _eventBus = eventBus;
            _objectRepo = objectRepo;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddReceivingDto addReceivingViewModel)
        {
            if (addReceivingViewModel == null || addReceivingViewModel.RegistrationToken.IsNullOrEmpty())
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var authResult = _ownershipAuthorization.IsAuthorized(tt => tt.Type == TokenType.Receiving && tt.Token == addReceivingViewModel.RegistrationToken,
                tt => tt.Registration.Object.OwnerUser);
            if (!authResult)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var theToken = (from t in _tokensRepo.Table
                            where t.Token == addReceivingViewModel.RegistrationToken && t.Type == TokenType.Receiving
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.INVALID",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var theRegistration = (from rg in _registrationsRepo.Table
                                   where rg.Tokens.Any(rt => rt.Token == addReceivingViewModel.RegistrationToken)
                                   select rg).FirstOrDefault();

            if (theRegistration is null)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            if (theRegistration.ObjectReceiving is object)
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var objectRegistrations = from rg in _registrationsRepo.Table
                                      where rg.ObjectId == theRegistration.ObjectId
                                      select rg;

            // if not all of them returned or not received
            if (!objectRegistrations.All(or => or.ObjectReceiving == null || or.ObjectReceiving.ObjectReturning != null))
            {
                return StatusCode(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.OBJECT.TAKEN",
                    Message = "Do you even have the object?",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            theToken.Status = TokenStatus.Used;

            var receiving = new ObjectReceiving
            {
                ReceivedAtUtc = DateTime.UtcNow,
                GiverLoginId = login.LoginId,
                RecipientLoginId = theToken.IssuerLoginId,
                HourlyCharge = 0f,
                ObjectRegistrationId = theRegistration.ObjectRegistrationId,
                ObjectReceivingId = Guid.NewGuid(),
            };

            _receivingsRepo.Add(receiving);
            // this will save theToken.Status also
            await _receivingsRepo.SaveChangesAsync();

            var evnt = new TransactionReceivedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                OccuredAt = DateTime.UtcNow,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                ReceivingId = receiving.ObjectReceivingId,
                RegistrationId = receiving.ObjectRegistrationId,
            };
            _eventBus.Publish(evnt);

            // Publish the event
            return StatusCode(200, new AddReceivingResultDto
            {
                ObjectId = _objectRepo.Get(theRegistration.ObjectId).OriginalObjectId,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                RegistrationId = theRegistration.ObjectRegistrationId,
                ShouldBeReturnedAfterReceving = theRegistration.ShouldReturnItAfter,
            });

        }
    }
}
