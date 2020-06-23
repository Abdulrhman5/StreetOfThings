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

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public class ObjectDeleter : IObjectDeleter
    {
        private CurrentUserCredentialsGetter _credentialsGetter;

        private IRepository<int, OfferedObject> _objectRepository;

        private ILogger<ObjectDeleter> _logger;
        public ObjectDeleter(CurrentUserCredentialsGetter credentialsGetter,
            IRepository<int, OfferedObject> objectsRepo,
            ILogger<ObjectDeleter> logger)
        {
            _credentialsGetter = credentialsGetter;
            _objectRepository = objectsRepo;
            _logger = logger;
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
            if (currentUser is null || currentUser.UserId != objectOwner.FirstOrDefault()?.OriginalUserId)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.UNAUTHORIZED",
                    Message = "You are unauthorized to delete this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            // Ask if there is a need to check the object is currently loaned

            var objectToDelete = _objectRepository.Get(objectDto.ObjectId);

            objectToDelete.ObjectStatus = ObjectStatus.Deleted;

            try
            {
                await _objectRepository.SaveChangesAsync();
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
    }
}
