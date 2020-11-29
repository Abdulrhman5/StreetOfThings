using ApplicationLogic.ProfilePhotoCommand;
using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommonLibrary;

namespace ApplicationLogic.AppUserQueries
{
    public class UserProfileGetter
    {
        private readonly IRepository<string, AppUser> _usersRepo;

        private ProfilePhotoUrlConstructor _urlConstructor;

        private CurrentUserCredentialsGetter _credentialsGetter;

        public UserProfileGetter(IRepository<string, AppUser> userRepo,
            ProfilePhotoUrlConstructor urlConstructor,
            CurrentUserCredentialsGetter credentialsGetter)
        {
            _usersRepo = userRepo;
            _urlConstructor = urlConstructor;
            _credentialsGetter = credentialsGetter;
        }

        public CommandResult<UserProfileDto> GetUserByIds()
        {
            var user = _credentialsGetter.GetCuurentUser();
            if(user == null) 
            {
                return new ErrorMessage
                {
                    ErrorCode = "USER.PROFILE.USER.UNKOWN",
                    Message = "Please login",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                }.ToCommand<UserProfileDto>();
            }
            var users = from u in _usersRepo.Table
                        where u.Id == user.UserId
                        select new UserProfileDto
                        {
                            Email = u.Email,
                            Name = u.NormalizedName,
                            PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                            Username = u.UserName,
                            Id = u.Id,
                            Gender = u.Gender.ToString(),
                            PhoneNumber = u.PhoneNumber
                        };

            var databaseUser = users.FirstOrDefault();
            if(databaseUser is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "USER.PROFILE.INTERNAL.ERROR",
                    Message = "There were an error while trying to get the user profile",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                }.ToCommand<UserProfileDto>();
            }
            return new CommandResult<UserProfileDto>(databaseUser);
        }

    }
}
