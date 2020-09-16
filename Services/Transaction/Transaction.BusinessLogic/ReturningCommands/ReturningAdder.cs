using CommonLibrary;
using EventBus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.ReturningCommands
{
    public interface IReturningAdder
    {
        public Task<CommandResult<ObjectReturningResultDto>> AddObjectReturning(ObjectReturningDto returnDto);
    }
    class ReturningAdder : IReturningAdder
    {
        private IRepository<Guid, ObjectReceiving> _receivingsRepo;

        private IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private IRepository<Guid, ObjectReturning> _returningRepo;

        private IRepository<Guid, TransactionToken> _tokensRepo;

        private IRepository<int, OfferedObject> _objectRepo;

        private OwnershipAuthorization<Guid, TransactionToken> _ownershipAuthorization;

        private readonly TransactionContext _transactionContext;

        private UserDataManager userDataManager;

        private IEventBus _eventBus;

        public ReturningAdder(IRepository<Guid, ObjectReceiving> receivingsRepo, IRepository<Guid, ObjectRegistration> registrationsRepo, IRepository<Guid, ObjectReturning> returningRepo, IRepository<Guid, TransactionToken> tokensRepo, IRepository<int, OfferedObject> objectRepo, OwnershipAuthorization<Guid, TransactionToken> ownershipAuthorization, TransactionContext transactionContext, UserDataManager userDataManager, IEventBus eventBus)
        {
            _receivingsRepo = receivingsRepo;
            _registrationsRepo = registrationsRepo;
            _returningRepo = returningRepo;
            _tokensRepo = tokensRepo;
            _objectRepo = objectRepo;
            _ownershipAuthorization = ownershipAuthorization;
            _transactionContext = transactionContext;
            this.userDataManager = userDataManager;
            _eventBus = eventBus;
        }

        public async Task<CommandResult<ObjectReturningResultDto>> AddObjectReturning(ObjectReturningDto returnDto)
        {
            if (returnDto == null || returnDto.ReturningToken.IsNullOrEmpty())
            {
                return new CommandResult<ObjectReturningResultDto>(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var (login, user) = await userDataManager.AddCurrentUserIfNeeded();
            if (login is null)
            {
                return new CommandResult<ObjectReturningResultDto>(new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.UNAUTHOROIZED",
                    Message = "You are not authorized to make this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var authResult = _ownershipAuthorization.IsAuthorized(tt => tt.Type == TokenType.Returning && tt.Token == returnDto.ReturningToken,
                tt => tt.Receiving.ObjectRegistration.Object.OwnerUser);
            if (!authResult)
            {
                return new CommandResult<ObjectReturningResultDto>(new ErrorMessage
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
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR code provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReturningResultDto>();
            }


            if (!(theToken.UseAfterUtc < DateTime.UtcNow && theToken.UseBeforeUtc > DateTime.UtcNow))
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.EXPIRED",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReturningResultDto>();
            }

            if (theToken.Status != TokenStatus.Ok)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.INVALID",
                    Message = "The QR code provided is too old",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReturningResultDto>();
            }

            var theReceiving = (from rc in _receivingsRepo.Table
                                where rc.Tokens.Any(returningToken => returningToken.Token == returnDto.ReturningToken)
                                select rc).Include(rc => rc.ObjectRegistration).FirstOrDefault();

            if (theReceiving is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.NOT.EXISTS",
                    Message = "The QR provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReturningResultDto>();
            }

            if (theReceiving.ObjectReturning is object)
            {
                return new ErrorMessage
                {
                    ErrorCode = "TRANSACTION.RETURNING.ADD.TOKEN.USED",
                    Message = "The QR code provided is already used",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectReturningResultDto>();
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
            var late = theReceiving.ReceivedAtUtc.Add(theReceiving.ObjectRegistration.ShouldReturnItAfter ?? new TimeSpan()) - DateTime.UtcNow;
            var charge = theReceiving.HourlyCharge is null ? null : (float?)(theReceiving.HourlyCharge * returnedAfter.TotalHours);
            return new CommandResult<ObjectReturningResultDto>(new ObjectReturningResultDto
            {
                RegistrationId = theReceiving.ObjectRegistrationId,
                ReceivingId = theReceiving.ObjectReceivingId,
                ReturningId = returning.ObjectReturningId,
                Late = late,
                ReturnedAfter = returnedAfter,
                ShouldPay = charge,
            });
        }
    }
}
