using HostingHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;

namespace Catalog.Infrastructure.Services
{
    public class ObjectPhotoService
    {
        private IImageSaver _imageSaver;

        private IRepository<int, OfferedObject> _objectsRepo;

        private IRepository<int, ObjectPhoto> _photoRepo;

        private OwnershipAuthorization<int, OfferedObject> _authorizer;

        public ObjectPhotoService(IImageSaver imageSaver, 
            IRepository<int, OfferedObject> objectsRepo,
            IRepository<int, ObjectPhoto> photoRepo, 
            OwnershipAuthorization<int, OfferedObject> authorizer)
        {
            _imageSaver = imageSaver;
            _objectsRepo = objectsRepo;
            _photoRepo = photoRepo;
            _authorizer = authorizer;
        }

        public async Task<CommandResult> AddPhotoToObject(int objectId, IFormFile image)
        {
            var authorizationResult = _authorizer.IsAuthorized(o => o.OfferedObjectId == objectId, (o) => o.OwnerLogin.User);

            if (!authorizationResult)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "OBJECT.PHOTO.UNAUTHORIZED",
                    Message = "You are not authorized to add a photo to this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }


            var @object = _objectsRepo.Get(objectId);
            if (@object is null || @object.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "OBJECT.DOES.NOT.EXISTS",
                    Message = "You are not authorized to add a photo to this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var savingResult = await _imageSaver.SaveImage(image);
            if (!savingResult.IsSuccessful)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = savingResult.Error.ErrorCode,
                    Message = savingResult.Error.Message,
                    StatusCode = savingResult.Error.StatusCode,
                });
            }

            var newPhoto = new ObjectPhoto
            {
                AddedAtUtc = DateTime.UtcNow,
                ObjectId = objectId,
                AdditionalInformation = QueryString.Create(new Dictionary<string, string>
                {
                    { "Name", savingResult.Result.Name.ToString() },
                    { "Version", "1" }
                }).ToUriComponent(),
                FilePath = savingResult.Result.Path,
            };

            _photoRepo.Add(newPhoto);
            await _photoRepo.SaveChangesAsync();

            return new CommandResult();
        }
    }
}
