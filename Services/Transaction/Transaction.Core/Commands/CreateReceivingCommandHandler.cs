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
using Transaction.Core.IntegrationEvents;
using EventBus;
using Transaction.Core.Exceptions;
using Transaction.Core.Extensions;

namespace Transaction.Core.Commands
{
    class CreateReceivingCommandHandler : IRequestHandler<CreateReceivingCommand, CommandResult<CreateReceivingResultDto>>
    {
        private IRepository<Guid, TransactionToken> _tokensRepo;
        private IUserDataManager userDataManager;
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;
        private IEventBus _eventBus;
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private IRepository<int, OfferedObject> _objectRepo;
        private IOwnershipAuthorization<Guid, TransactionToken> _ownershipAuthorization;

        public CreateReceivingCommandHandler(IRepository<Guid, TransactionToken> tokensRepo,
            IUserDataManager userDataManager,
            IRepository<Guid, ObjectReceiving> receivingsRepo,
            IEventBus eventBus,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IRepository<int, OfferedObject> objectRepo,
            IOwnershipAuthorization<Guid, TransactionToken> ownershipAuthorization)
        {
            _tokensRepo = tokensRepo;
            this.userDataManager = userDataManager;
            _receivingsRepo = receivingsRepo;
            _eventBus = eventBus;
            _registrationsRepo = registrationsRepo;
            _objectRepo = objectRepo;
            _ownershipAuthorization = ownershipAuthorization;
        }

        public async Task<CommandResult<CreateReceivingResultDto>> Handle(CreateReceivingCommand request, CancellationToken cancellationToken)
        {
            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<CreateReceivingResultDto>();
            }

            var authResult = _ownershipAuthorization.IsAuthorized(tt => tt.Type == TokenType.Receiving && tt.Token == request.RegistrationToken,
                tt => tt.Registration.Object.OwnerUser);
            if (!authResult)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<CreateReceivingResultDto>();
            }

            var theToken = (from t in _tokensRepo.Table
                            where t.Token == request.RegistrationToken && t.Type == TokenType.Receiving
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReceivingResultDto>();
            }

            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReceivingResultDto>();
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.INVALID",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReceivingResultDto>();
            }

            var theRegistration = (from rg in _registrationsRepo.Table
                                   where rg.Tokens.Any(rt => rt.Token == request.RegistrationToken)
                                   select rg).FirstOrDefault();

            if (theRegistration is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReceivingResultDto>();
            }


            if (theRegistration.ObjectReceiving is object)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RECEIVING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReceivingResultDto>();
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
                }.ToCommand<CreateReceivingResultDto>();
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
            return new CommandResult<CreateReceivingResultDto>(new CreateReceivingResultDto
            {
                ObjectId = _objectRepo.Get(theRegistration.ObjectId).OriginalObjectId,
                ReceivedAtUtc = receiving.ReceivedAtUtc,
                RegistrationId = theRegistration.ObjectRegistrationId,
                ShouldBeReturnedAfterReceving = theRegistration.ShouldReturnItAfter,
            });
        }
    }
}
