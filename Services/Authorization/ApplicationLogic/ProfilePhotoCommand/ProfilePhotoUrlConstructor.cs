using Microsoft.AspNetCore.Http;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using DataAccessLayer;
using System.Linq;
using System.Web;

namespace ApplicationLogic.ProfilePhotoCommand
{
    public class ProfilePhotoUrlConstructor
    {
        private string _domainName;

        private IRepository<int, ProfilePhoto> _photosRepo;
        public ProfilePhotoUrlConstructor(IHttpContextAccessor httpContextAccessor, IRepository<int, ProfilePhoto> photosRepo)
        {
            _domainName = new Uri(httpContextAccessor.HttpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority);
            _photosRepo = photosRepo;
        }

        public string ConstructOrDefault(ProfilePhoto profilePhoto)
        {
            if(profilePhoto == null)
            {
                return $"{_domainName}/favicon.ico";
            }
            return $"{_domainName}/Resources/Photo/Profile/{HttpUtility.ParseQueryString(profilePhoto.AdditionalInformation)["Name"]}";
        }

        public string ConstructOrDefaultForUser(AppUser user)
        {
            if(user == null)
            {
                return $"{_domainName}/favicon.ico";
            }
            else
            {

                var profilePhoto = (from p in _photosRepo.Table
                                    where p.UserId == user.Id
                                    orderby p.AddedAtUtc descending
                                    select p).LastOrDefault();

                return ConstructOrDefault(profilePhoto);
            }
        }
    }
}
