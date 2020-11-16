using EventBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Core.Events;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Service.Dtos;

namespace Transaction.Core.Commands
{
    public class CreateRegistrationCommandHandler : IRequestHandler<CreateRegistrationCommand, CreateRegistrationResultDto>
    {
        private IRepository<Guid, ObjectRegistration> _registrationsRepo;
        private ITransactionTokenManager _transactionTokenManager;
        private IObjectDataManager _objectsRepo;
        private IUserDataManager _userDataManager;
        private IEventBus _eventBus;

        private int maximumHoursForFreeLending;

        private int maximumHoursForReservationExpiration;


        public CreateRegistrationCommandHandler(IRepository<Guid, ObjectRegistration> registrationsRepo,
            ITransactionTokenManager transactionTokenManager,
            IObjectDataManager objectsRepo,
            IUserDataManager userDataManager,
            IConfiguration configuration,
            IEventBus eventBus)
        {
            _registrationsRepo = registrationsRepo;
            _transactionTokenManager = transactionTokenManager;
            _objectsRepo = objectsRepo;
            _userDataManager = userDataManager;

            maximumHoursForFreeLending = int.Parse(configuration["Registration:MaximumHoursForFreeLending"]);
            maximumHoursForReservationExpiration = int.Parse(configuration["Registration:MaximumHoursForRegistrationExpiration"]);
            _eventBus = eventBus;
        }

        public async Task<CreateRegistrationResultDto> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            var @object = await _objectsRepo.GetObjectAsync(request.ObjectId);

            TimeSpan? shouldReturnItAfter;
            if (@object.ShouldReturn)
            {
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

            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            var registrationModel = new ObjectRegistration
            {
                ObjectRegistrationId = Guid.NewGuid(),
                RegisteredAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(maximumHoursForReservationExpiration),
                ObjectId = @object.OfferedObjectId,
                Status = ObjectRegistrationStatus.OK,
                RecipientLoginId = login.LoginId,
                ShouldReturnItAfter = shouldReturnItAfter,
            };

            _registrationsRepo.Add(registrationModel);
            await _registrationsRepo.SaveChangesAsync();


            var integrationEvent = new NewRegistrationIntegrationEvent()
            {
                Id = Guid.NewGuid(),
                OccuredAt = registrationModel.RegisteredAtUtc,
                ObjectId = @object.OriginalObjectId,
                RecipiantId = user.UserId.ToString(),
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

            return dto;
        }
    }
}
