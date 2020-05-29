using CommonLibrary;
using DataAccessLayer;
using HostingHelpers;
using Microsoft.AspNetCore.Http;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.ProfilePhotoCommand
{
    public class ProfilePhotoSaver
    {
        private IImageSaver _imageSaver;

        private CurrentUserCredentialsGetter _userCredentialsGetter;

        private IRepository<int, ProfilePhoto> _profileRepo;

        public ProfilePhotoSaver(IImageSaver imageSaver, 
            CurrentUserCredentialsGetter userCredentialsGetter,
            IRepository<int, ProfilePhoto> profileRepo)
        {
            _imageSaver = imageSaver;
            _userCredentialsGetter = userCredentialsGetter;
            _profileRepo = profileRepo;
        }

        public async Task<CommandResult> SaveImage(IFormFile image)
        {
            var savingResult = await _imageSaver.SaveImage(image);

            if(!savingResult.IsSuccessful)
            {
                return new CommandResult(savingResult.Error);
            }

            var currentUser = _userCredentialsGetter.GetCuurentUser();

            var photo = new ProfilePhoto
            {
                AddedAtUtc = DateTime.UtcNow,
                UserId = currentUser.UserId,
                FilePath = savingResult.Result.Path,
                AdditionalInformation = QueryString.Create(new Dictionary<string, string>
                {
                    {"Name",savingResult.Result.Name.ToString() },
                    {"Version","1" }
                }).ToUriComponent(),
            };

            _profileRepo.Add(photo);
            await _profileRepo.SaveChangesAsync();
            return new CommandResult();
        }
    }
}
