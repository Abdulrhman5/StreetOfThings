using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using HostingHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommonLibrary;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public class PhotoAdder
    {
        private IImageSaver _imageSaver;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private IRepository<int, OfferedObject> _objectsRepo;

        private IRepository<int, ObjectPhoto> _photoRepo;

        public PhotoAdder(IImageSaver imageSaver, 
            CurrentUserCredentialsGetter credentialsGetter,
            IRepository<int, OfferedObject> objectsRepo,
            IRepository<int, ObjectPhoto> photoRepo)
        {
            _imageSaver = imageSaver;
            _credentialsGetter = credentialsGetter;
            _objectsRepo = objectsRepo;
            _photoRepo = photoRepo;
        }


        public async Task<CommandResult> AddPhotoToObject(int objectId, IFormFile image)
        {
            var currentUser = _credentialsGetter.GetCuurentUser();

            var objectBelongToCurrentUser = (from o in _objectsRepo.Table
                      where o.OfferedObjectId == objectId && o.Owner.OriginalUserId == currentUser.UserId
                      select o).Any();

            if (!objectBelongToCurrentUser)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "OBJECT.PHOTO.UNAUTHORIZED",
                    Message = "You are not authorized to add a photo to this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }


            var @object = _objectsRepo.Get(objectId);
            if(@object is null || @object.ObjectStatus != ObjectStatus.Available)
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
                return new CommandResult(savingResult.Error);
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
