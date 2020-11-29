using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Core.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Infrastructure.GrpcClients;

namespace Transaction.Infrastructure.Services
{
    class UserDataManager : IUserDataManager
    {
        private IRepository<Guid, User> _userRepo;

        private IRepository<Guid, Login> _loginRepo;

        private UsersGrpcClient _loginInformationGetter;

        HttpContext _httpContext;

        public UserDataManager(IRepository<Guid, User> userRepo,
            IRepository<Guid, Login> loginRepo,
            UsersGrpcClient loginInformationGetter,
            IHttpContextAccessor accessor)
        {
            _userRepo = userRepo;
            _loginRepo = loginRepo;
            _loginInformationGetter = loginInformationGetter;
            _httpContext = accessor.HttpContext;
        }

        public virtual async Task<(Login, User)> AddUserIfNotExisted(Guid tokenId, string originUserId, string accessToken)
        {

            // if The user existed but the login does not exist
            var login = _loginRepo.Table.Include(login => login.User).SingleOrDefault(t => t.LoginId == tokenId);
            if (login is null)
            {
                // add the login 
                var loginInformation = await _loginInformationGetter.GetLoginInformation(tokenId.ToString());
                var userIdGuid = Guid.Parse(loginInformation.UserId);
                var theUser = _userRepo.Table.SingleOrDefault(u => u.UserId == userIdGuid);
                if (theUser is null)
                {
                    var userToBeAdded = new User
                    {
                        UserId = userIdGuid,
                        Status = UserStatus.Available,
                        UserName = loginInformation.Username,
                        Logins = new List<Login>
                        {
                            new Login
                            {
                                LoginId = Guid.Parse(loginInformation.TokenId),
                            }
                        }
                    };

                    _userRepo.Add(userToBeAdded);
                    await _userRepo.SaveChangesAsync();

                    return (userToBeAdded.Logins.FirstOrDefault(), userToBeAdded);
                }
                else
                {
                    var loginToBeAdded = new Login
                    {
                        LoginId = Guid.Parse(loginInformation.TokenId),
                        UserId = theUser.UserId,
                    };

                    _loginRepo.Add(loginToBeAdded);
                    await _loginRepo.SaveChangesAsync();
                    return (loginToBeAdded, theUser);
                }
            }
            else
            {
                return (login, login.User);
            }
        }

        public virtual async Task<(Login, User)> AddCurrentUserIfNeeded()
        {
            var credentials = GetCurrentUser();
            if (credentials is null)
            {
                return (null, null);
            }
            else
            {
                return await AddUserIfNotExisted(Guid.Parse(credentials.TokenId), credentials.UserId, credentials.AccessToken);
            }
        }

        public async Task<User> AddUserIfNeeded(string userId)
        {
            var usersById = (from u in _userRepo.Table
                             where u.UserId.ToString() == userId
                             select u).FirstOrDefault();

            if (usersById is object)
            {
                return usersById;
            }

            var user = await _loginInformationGetter.GetUser(userId);
            var userToBeAdded = new User
            {
                UserId = Guid.Parse(userId),
                Status = UserStatus.Available,
                UserName = user.Username,
            };

            _userRepo.Add(userToBeAdded);
            await _userRepo.SaveChangesAsync();

            return userToBeAdded;
        }

        public LoginDataDto GetCurrentUser()
        {
            return new LoginDataDto
            {
                AccessToken = _httpContext.Request.Headers["Authorization"],
                TokenId = _httpContext.User.FindFirst("TokenId").Value,
                UserId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
            };
        }

    }
}
