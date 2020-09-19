using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Catalog.ApplicationLogic.Events;
using EventBus;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public class ObjectDeleter : IObjectDeleter
    {
        private CurrentUserCredentialsGetter _credentialsGetter;

        private IRepository<int, OfferedObject> _objectRepository;

        private ILogger<ObjectDeleter> _logger;

        private IEventBus _eventBus;
        public ObjectDeleter(CurrentUserCredentialsGetter credentialsGetter,
            IRepository<int, OfferedObject> objectsRepo,
            ILogger<ObjectDeleter> logger,
            IEventBus eventBus)
        {
            _credentialsGetter = credentialsGetter;
            _objectRepository = objectsRepo;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<CommandResult> AuthorizedDelete(DeleteObjectDto objectDto)
        {
            if (objectDto is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var objectToDelete = _objectRepository.Get(objectDto.ObjectId);

            if (objectToDelete is null || objectToDelete.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.NOTFOUND",
                    Message = "The obect you are trying to delete does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            objectToDelete.ObjectStatus = ObjectStatus.Deleted;

            try
            {
                await _objectRepository.SaveChangesAsync();
                var integrationEvent = new ObjectDeletedIntegrationEvent
                {
                    Id = Guid.NewGuid(),
                    ObjectId = objectToDelete.OfferedObjectId,
                    DeletedAtUtc = DateTime.UtcNow,
                    OccuredAt = DateTime.UtcNow
                };
                _eventBus.Publish(integrationEvent);
                return new CommandResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"There were a problem deleting the object:{objectDto.ObjectId}");
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.INTERNAL.ERROR",
                    Message = "There were an error deleting your object",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }

        }


        public async Task<CommandResult> DeleteObject(DeleteObjectDto objectDto)
        {
            if(objectDto is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            var objectOwner = from o in _objectRepository.Table
                              where o.OfferedObjectId == objectDto.ObjectId
                              select o.OwnerLogin.User;

            var currentUser = _credentialsGetter.GetCuurentUser();
            if (currentUser is null || currentUser.UserId != objectOwner.FirstOrDefault()?.UserId.ToString())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.UNAUTHORIZED",
                    Message = "You are unauthorized to delete this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }
            return await AuthorizedDelete(objectDto);
        }
    }
}
