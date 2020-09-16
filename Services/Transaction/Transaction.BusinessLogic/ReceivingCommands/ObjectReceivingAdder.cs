using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Cryptography;
using EventBus;
using Transaction.BusinessLogic.Events;

namespace Transaction.BusinessLogic.ReceivingCommands
{
    class ObjectReceivingAdder : IObjectReceivingAdder
    {
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private IRepository<Guid, TransactionToken> _tokensRepo;

        private IRepository<int, OfferedObject> _objectRepo;

        private OwnershipAuthorization<Guid, TransactionToken> _ownershipAuthorization;

        private readonly TransactionContext _transactionContext;

        private UserDataManager userDataManager;

        private IEventBus _eventBus;

        public ObjectReceivingAdder(IRepository<Guid, ObjectReceiving> receivingsRepo,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IRepository<Guid, TransactionToken> tokensRepo,
            OwnershipAuthorization<Guid, TransactionToken> ownershipAuthorization,
            TransactionContext transactionContext,
            UserDataManager userDataManager,
            IRepository<int, OfferedObject> objectRepo, 
            IEventBus eventBus)
        {
            _receivingsRepo = receivingsRepo;
            _registrationsRepo = registrationsRepo;
            _tokensRepo = tokensRepo;
            _ownershipAuthorization = ownershipAuthorization;
            _transactionContext = transactionContext;
            this.userDataManager = userDataManager;
            _objectRepo = objectRepo;
            _eventBus = eventBus;
        }

        public async Task<CommandResult<ObjectReceivingResultDto>> AddReceiving(AddReceivingDto addReceivingDto)
        {
            if (addReceivingDto == null || addReceivingDto.RegistrationToken.IsNullOrEmpty())
            {
                return new CommandResult<ObjectReceivingResultDto>(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return new CommandResult<ObjectReceivingResultDto>(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var authResult = _ownershipAuthorization.IsAuthorized(tt => tt.Type == TokenType.Receiving && tt.Token == addReceivingDto.RegistrationToken,
                tt => tt.Registration.Object.OwnerUser);
            if (!authResult)
            {
                return new CommandResult<ObjectReceivingResultDto>(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var theToken = (from t in _tokensRepo.Table
                            where t.Token == addReceivingDto.RegistrationToken && t.Type == TokenType.Receiving
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
            }

            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.INVALID",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
            }

            var theRegistration = (from rg in _registrationsRepo.Table
                                   where rg.Tokens.Any(rt => rt.Token == addReceivingDto.RegistrationToken)
                                   select rg).FirstOrDefault();

            if (theRegistration is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
            }


            if (theRegistration.ObjectReceiving is object)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
            }

            var objectRegistrations = from rg in _registrationsRepo.Table
                                      where rg.ObjectId == theRegistration.ObjectId
                                      select rg;

            // if not all of them returned or not received
            if (!objectRegistrations.All(or => or.ObjectReceiving == null || or.ObjectReceiving.ObjectReturning != null))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.OBJECT.TAKEN",
                    Message = "Do you even have the object?",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReceivingResultDto>();
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
                ObjectReturningId = null,
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
            return new CommandResult<ObjectReceivingResultDto>(new ObjectReceivingResultDto
            {
                ObjectId = _objectRepo.Get(theRegistration.ObjectId).OriginalObjectId,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                RegistrationId = theRegistration.ObjectRegistrationId,
                ShouldBeReturnedAfterReceving = theRegistration.ShouldReturnItAfter,
            });
        }
    }
}
