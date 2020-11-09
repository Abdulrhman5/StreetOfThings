using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Catalog.ApplicationCore.Extensions;

namespace Catalog.ApplicationCore.Services
{
    class ObjectService : IObjectService
    {

        private IRepository<int, OfferedObject> _objectRepo;

        private ILogger<ObjectService> _logger;

        IUserDataManager _userDataManager;

        private IRepository<int, Tag> _tagRepo;

        public ObjectService(IUserDataManager userDataManager,
            IRepository<int, OfferedObject> objectsRepo,
            ILogger<ObjectService> logger,
            IRepository<int, Tag> tagRepo)
        {
            _userDataManager = userDataManager;
            _objectRepo = objectsRepo;
            _logger = logger;
            _tagRepo = tagRepo;
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

            var objectToDelete = _objectRepo.Get(objectDto.ObjectId);

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
                await _objectRepo.SaveChangesAsync();
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
            if (objectDto is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            var objectOwner = from o in _objectRepo.Table
                              where o.OfferedObjectId == objectDto.ObjectId
                              select o.OwnerLogin.User;

            var currentUser = _userDataManager.GetCuurentUser();
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

        public async Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto)
        {
            if (objectDto is null)
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.NULL",
                    Message = "Please fill the field with data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (objectDto.ObjectName.IsNullOrEmpty())
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.OBJECTNAME.EMPTY",
                    Message = "Please fill the 'object name' field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            // Description could be null

            if (objectDto.Tags is null || objectDto.Tags.Count < 2)
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.TAGS.TOO.FEW",
                    Message = "Please add at least two tags",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var invalidTags = from t in objectDto.Tags
                              where t.IsNullOrEmpty() || t.Length < 4
                              select t;

            if (invalidTags.Any())
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.TAGS.INVALID",
                    Message = "Please send a valid tags",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            var alreadyExistedTags = (from t in _tagRepo.Table
                                      where objectDto.Tags.Any(tt => tt == t.Name) && t.TagStatus == TagStatus.Ok
                                      select t).ToList();

            // No Logic for not existed tags, just discard them.

            var objectTags = alreadyExistedTags.Select(t => new ObjectTag
            {
                TagId = t.TagId
            }).ToList();

            var (login, ownerUser) = await _userDataManager.AddCurrentUserIfNeeded();
            var @object = new OfferedObject
            {
                Description = objectDto.Description,
                Name = objectDto.ObjectName,
                PublishedAt = DateTime.UtcNow,
                Tags = objectTags,
                OwnerLoginId = login.LoginId,
                CurrentTransactionType = objectDto.Type
            };


            _objectRepo.Add(@object);
            _objectRepo.SaveChanges();
            return new CommandResult<OfferedObject>(@object);
        }
    }
}
