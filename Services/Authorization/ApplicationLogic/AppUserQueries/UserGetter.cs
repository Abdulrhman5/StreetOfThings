using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.ProfilePhotoCommand;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;

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

        public async  Task<List<UserForAdministrationDto>> GetUsersAsync(PagingArguments args)
        {
            var x = from u in _usersRepo.Table
                    select new UserForAdministrationDto
                    {
                        Id = Guid.Parse(u.Id),
                        Email = u.Email,
                        Gender = u.Gender.ToString(),
                        IsEmailConfirmed = u.EmailConfirmed,
                        PhoneNumber = u.PhoneNumber,
                        RegisteredAt = u.CreatedAt,
                        Username = u.UserName,
                        AccessFeildCount = u.AccessFailedCount,
                        PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                    };
            return await x.SkipTakeAsync(args);
        }

        public async Task<List<UserForAdministrationDto>> GetUsersAsync()
        {
            var x = from u in _usersRepo.Table
                    select new UserForAdministrationDto
                    {
                        Id = Guid.Parse(u.Id),
                        Email = u.Email,
                        Gender = u.Gender.ToString(),
                        IsEmailConfirmed = u.EmailConfirmed,
                        PhoneNumber = u.PhoneNumber,
                        RegisteredAt = u.CreatedAt,
                        Username = u.UserName,
                        AccessFeildCount = u.AccessFailedCount,
                        PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                    };
            return await x.ToListAsync();
        }

        public async Task<UserListDto> GetUsersWithStatsAsync()
        {
            var stats = (from u in _usersRepo.Table
                        group u by 1 into g
                        select new
                        {
                            Male = _usersRepo.Table.Where(u => u.Gender == Gender.Male).Count(),
                            Female = _usersRepo.Table.Where(u => u.Gender == Gender.Female).Count(),
                            Blocked = 0,
                            All = _usersRepo.Table.Count()
                        }).FirstOrDefault();

            var x = await (from u in _usersRepo.Table
                    select new UserForAdministrationDto
                    {
                        Id = Guid.Parse(u.Id),
                        Email = u.Email,
                        Gender = u.Gender.ToString(),
                        IsEmailConfirmed = u.EmailConfirmed,
                        PhoneNumber = u.PhoneNumber,
                        RegisteredAt = u.CreatedAt,
                        Username = u.UserName,
                        AccessFeildCount = u.AccessFailedCount,
                        PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                    }).ToListAsync();

            var result = new UserListDto
            {
                AllUserCount = stats.All,
                BlockedUsersCount = stats.Blocked,
                FemaleUsersCount = stats.Female,
                MaleUsersCount = stats.Male,
                Users = x,
            };

            return result;
        }
    }
}
