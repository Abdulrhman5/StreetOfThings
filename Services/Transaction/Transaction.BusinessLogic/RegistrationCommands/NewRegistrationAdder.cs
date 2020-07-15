using CommonLibrary;
using EventBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Events;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    class NewRegistrationAdder : INewRegistrationAdder
    {
        private readonly UserDataManager _userDataManager;

        private readonly IRepository<Guid, ObjectRegistration> _registrationsRepo;

        private readonly ObjectDataManager _objectDataManager;

        private readonly IRepository<Guid, ObjectReceiving> _objectReceiving;

        private IEventBus _eventBus;

        private readonly ITransactionTokenManager _tokenManager;

        private IConfiguration _configurations;

        private int maximumHoursForFreeLending = 6;

        private int maximumHoursForReservationExpiration = 24;
        public NewRegistrationAdder(UserDataManager userDataManager,
            ObjectDataManager objectDataManager,
            IRepository<Guid, ObjectRegistration> registrationsRepo,
            IRepository<Guid, ObjectReceiving> receivingsRepo, 
            IEventBus eventBus, 
            ITransactionTokenManager tokenManager,
            IConfiguration configuration)
        {
            _userDataManager = userDataManager;

            _registrationsRepo = registrationsRepo;
            _objectReceiving = receivingsRepo;

            _objectDataManager = objectDataManager;

            _eventBus = eventBus;
            _tokenManager = tokenManager;
            _configurations = configuration;

            maximumHoursForFreeLending = _configurations.GetValue<int>("Registration:MaximumHoursForFreeLending");
            maximumHoursForReservationExpiration = _configurations.GetValue<int>("Registration:MaximumHoursForRegistrationExpiration");
        }

        private ErrorMessage ObjectNotAvailable = new ErrorMessage
        {
            ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AVAILABLE",
            Message = "This object is not available",
            StatusCode = System.Net.HttpStatusCode.BadRequest
        };

         
        public async Task<CommandResult<ObjectRegistrationDto>> AddNewRegistrationAsync(AddNewRegistrationDto newRegistrationDto)
        {
            var user = await _userDataManager.AddCurrentUserIfNeeded();
            if (user.Login == null)
            {
                return new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this operation",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<ObjectRegistrationDto>();
            }

            if (newRegistrationDto is null)
            {
                return new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NULL",
                    Message = "Please send a valid information",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectRegistrationDto>();
            }

            var @object = await _objectDataManager.GetObjectAsync(newRegistrationDto.ObjectId);
            if (@object is null)
            {
                return new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.EXISTS",
                    Message = "The object specified does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectRegistrationDto>();
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
                    return ObjectNotAvailable.ToCommand<ObjectRegistrationDto>();
                }
            }

            // See Previous registrations

            var existingRegistrations = from registration in _registrationsRepo.Table
                                        where registration.RecipientLogin.UserId == user.User.UserId && registration.ObjectId == @object.OfferedObjectId
                                        select registration;

            // If The user taken and has this object OR If the user has another registeration pending receiving
            if (existingRegistrations.Any(reg => reg.ObjectReceiving == null || reg.ObjectReceiving.ObjectReturning == null))
            {
                return ObjectNotAvailable.ToCommand<ObjectRegistrationDto>();
            }

            TimeSpan? shouldReturnItAfter;
            if (@object.ShouldReturn)
            {
                // If the object should return but the user has not specified the time he should return the object
                if (!newRegistrationDto.ShouldReturnAfter.HasValue)
                {
                    return new ErrorMessage
                    {
                        ErrorCode = "TRANSACTION.OBJECT.RESERVE.SHOULDRETURN.NULL",
                        Message = "Please specify when you will return this object",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    }.ToCommand<ObjectRegistrationDto>();
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
                RecipiantId = user.User.OriginalUserId,
                ShouldReturn = @object.ShouldReturn
            };

            // Broadcast an event;
            _eventBus.Publish(integrationEvent);


            var token = await _tokenManager.GenerateToken(registrationModel.ObjectRegistrationId, TokenType.Receiving);

            var dto = new ObjectRegistrationDto
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

            return new CommandResult<ObjectRegistrationDto>(dto);
        }
    }
}
