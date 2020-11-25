using EventBus;
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
using Transaction.Core.IntegrationEvents;

namespace Transaction.Core.Commands
{
    class CreateReturningCommandHandler : IRequestHandler<CreateReturningCommand, CommandResult<CreateReturningResultDto>>
    {
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;
        private IRepository<Guid, ObjectReturning> _returningRepo;
        private IRepository<Guid, TransactionToken> _tokensRepo;
        private IOwnershipAuthorization<Guid, TransactionToken> _transactionOwnershipAuthorizer;
        private IUserDataManager _userDataManager;
        private IEventBus _eventBus;
        private IQueryableHelper _queryHelper;

        public CreateReturningCommandHandler(IRepository<Guid, ObjectReceiving> receivingsRepo,
            IRepository<Guid, ObjectReturning> returningRepo,
            IRepository<Guid, TransactionToken> tokensRepo,
            IOwnershipAuthorization<Guid, TransactionToken> transactionOwnershipAuthorizer,
            IUserDataManager userDataManager,
            IEventBus eventBus,
            IQueryableHelper queryHelper)
        {
            _receivingsRepo = receivingsRepo;
            _returningRepo = returningRepo;
            _tokensRepo = tokensRepo;
            _transactionOwnershipAuthorizer = transactionOwnershipAuthorizer;
            _userDataManager = userDataManager;
            _eventBus = eventBus;
            _queryHelper = queryHelper;
        }

        public async Task<CommandResult<CreateReturningResultDto>> Handle(CreateReturningCommand request, CancellationToken cancellationToken)
        {
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<CreateReturningResultDto>();
            }

            var authResult = _transactionOwnershipAuthorizer.IsAuthorized(tt => tt.Type == TokenType.Returning && tt.Token == request.ReturningToken,
                tt => tt.Receiving.ObjectRegistration.Object.OwnerUser);
            if (!authResult)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<CreateReturningResultDto>();
            }


            var theToken = (from t in _tokensRepo.Table
                            where t.Token == request.ReturningToken && t.Type == TokenType.Returning
                            select t).FirstOrDefault();

            if (theToken == null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReturningResultDto>();
            }

            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReturningResultDto>();
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReturningResultDto>();
            }

            var theReceiving = (from rc in _receivingsRepo.Table
                                where rc.Tokens.Any(returningToken => returningToken.Token == request.ReturningToken)
                                select rc).Include(rc => rc.ObjectRegistration,_queryHelper).FirstOrDefault();

            if (theReceiving is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReturningResultDto>();
            }

            if (theReceiving.ObjectReturning is object)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateReturningResultDto>();
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

            return new CommandResult<CreateReturningResultDto>(new CreateReturningResultDto
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
