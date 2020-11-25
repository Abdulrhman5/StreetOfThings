using EventBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Events;
using Transaction.Core.Exceptions;
using Transaction.Core.Extensions;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Service.Dtos;
using System.Linq;

namespace Transaction.Core.Commands
{
    public class CreateRegistrationCommandHandler : IRequestHandler<CreateRegistrationCommand, CommandResult<CreateRegistrationResultDto>>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private ITransactionTokenManager _transactionTokenManager;
        private IObjectDataManager _objectsRepo;
        private IUserDataManager _userDataManager;
        private IEventBus _eventBus;

        private int maximumHoursForFreeLending;

        private int maximumHoursForReservationExpiration;
        private IRepository<Guid,ObjectReceiving> _objectReceiving;

        public CreateRegistrationCommandHandler(IRepository<Guid, ObjectRegistration> registrationsRepo,
            ITransactionTokenManager transactionTokenManager,
            IObjectDataManager objectsRepo,
            IUserDataManager userDataManager,
            IConfiguration configuration,
            IEventBus eventBus,
            IRepository<Guid, ObjectReceiving> objectReceiving)
        {
            _registrationsRepo = registrationsRepo;
            _transactionTokenManager = transactionTokenManager;
            _objectsRepo = objectsRepo;
            _userDataManager = userDataManager;

            maximumHoursForFreeLending = int.Parse(configuration["Registration:MaximumHoursForFreeLending"]);
            maximumHoursForReservationExpiration = int.Parse(configuration["Registration:MaximumHoursForRegistrationExpiration"]);
            _eventBus = eventBus;
            _objectReceiving = objectReceiving;
        }

        public async Task<CommandResult<CreateRegistrationResultDto>> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
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
                return new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this operation",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<CreateRegistrationResultDto>();
            }

            var @object = await _objectsRepo.GetObjectAsync(request.ObjectId);
            if (@object is null)
            {
                return new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.EXISTS",
                    Message = "The object specified does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<CreateRegistrationResultDto>();
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
                    return ObjectNotAvailable.ToCommand<CreateRegistrationResultDto>();
                }
            }

            // See Previous registrations

            var existingRegistrations = from registration in _registrationsRepo.Table
                                        where registration.RecipientLogin.UserId == user.User.UserId && registration.ObjectId == @object.OfferedObjectId
                                        select registration;

            // If The user taken and has this object OR If the user has another registeration pending receiving
            if (existingRegistrations.Any(reg => reg.ObjectReceiving == null || reg.ObjectReceiving.ObjectReturning == null))
            {
                return ObjectNotAvailable.ToCommand<CreateRegistrationResultDto>();
            }


            TimeSpan? shouldReturnItAfter;
            if (@object.ShouldReturn)
            {
                // If the object should return but the user has not specified the time he should return the object
                if (!request.ShouldReturnAfter.HasValue)
                {
                    return new ErrorMessage
                    {
                        ErrorCode = "TRANSACTION.OBJECT.RESERVE.SHOULDRETURN.NULL",
                        Message = "Please specify when you will return this object",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    }.ToCommand<CreateRegistrationResultDto>();
                }

                if (@object.HourlyCharge.HasValue)
                {
                    shouldReturnItAfter = new TimeSpan(request.ShouldReturnAfter.Value, 0, 0);
                }
                else
                {
                    if (request.ShouldReturnAfter > maximumHoursForFreeLending)
                        shouldReturnItAfter = new TimeSpan(maximumHoursForFreeLending, 0, 0);
                    else
                        shouldReturnItAfter = new TimeSpan(request.ShouldReturnAfter.Value, 0, 0);
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


            var token = await _transactionTokenManager.GenerateToken(registrationModel.ObjectRegistrationId, TokenType.Receiving);

            var dto = new CreateRegistrationResultDto
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

            return new CommandResult<CreateRegistrationResultDto>(dto);
        }
    }
}
