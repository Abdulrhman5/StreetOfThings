using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.ProfilePhotoCommand;
using AuthorizationService;
using DataAccessLayer;
using Grpc.Core;
using Models;
using static AuthorizationService.UserDirectory;

namespace AuthorizationService.Grpc
{
    public class UserService : UserDirectoryBase
    {
        private IRepository<string, AppUser> _usersRepo;

        private ProfilePhotoUrlConstructor _urlConstructor;
        public UserService (IRepository<string , AppUser> usersRepo)
        {
            _usersRepo = usersRepo;
        }

        public override async Task<UsersModel> GetUsersData(UsersIds request, ServerCallContext context)
        {
            var users = from u in _usersRepo.Table
                        where request.UserId.Any(i => i == u.Id)
                        select new UserModel
                        {
                            Email = u.Email,
                            NormializedName = u.NormalizedName,
                            PictureUrl = _urlConstructor.ConstructOrDefault(u.Photos.OrderByDescending(pp => pp.AddedAtUtc).FirstOrDefault()),
                            Username = u.UserName,
                        };

            var model =  new UsersModel();
            model.Users.AddRange(users);
            return model;
        }
    }
}
