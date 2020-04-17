using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    class UserGetter : IUserService
    {
        private readonly IRepository<string, AppUser> _usersRepository;

        public UserGetter (IRepository<string, AppUser> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public IEnumerable<UserForAdministrationDto> GetUsers()
        {
            var x = from u in _usersRepository.Table
                    select new UserForAdministrationDto
                    {
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

    }
}
