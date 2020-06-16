using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.ProfilePhotoCommand;
using CommonLibrary;

namespace ApplicationLogic.AppUserQueries
{
    class UserGetter : IUserGetter
    {
        private readonly IRepository<string, AppUser> _usersRepo;

        private ProfilePhotoUrlConstructor _urlConstructor;

        public UserGetter (IRepository<string, AppUser> usersRepository, ProfilePhotoUrlConstructor urlConstructor)
        {
            _usersRepo = usersRepository;
            _urlConstructor = urlConstructor;
        }

        public IEnumerable<UserForAdministrationDto> GetUsers()
        {
            var x = from u in _usersRepo.Table
                    select new UserForAdministrationDto
                    {
                        Id= Guid.Parse(u.Id),
                        Email = u.Email,
                        Gender = u.Gender.ToString(),
                        IsEmailConfirmed = u.EmailConfirmed,
                        PhoneNumber = u.PhoneNumber,
                        RegisteredAt = u.CreatedAt,
                        Username = u.UserName,
                        AccessFeildCount = u.AccessFailedCount,
                    };
            return x;
        }

        public IEnumerable<UserDto> GetUserByIds(List<string> usersId)
        {
            if(usersId.IsNullOrEmpty())
            {
                return new List<UserDto>();
            }

            var users = from u in _usersRepo.Table
                        where usersId.Any(i => i == u.Id)
                        select new UserDto
                        {
                            Email = u.Email,
                            Name = u.NormalizedName,
                            PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                            Username = u.UserName,
                            Id = u.Id
                        };

            return users;
        }

    }
}
