using Catalog.ApplicationLogic.GrpcClients;
using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    class UserDataManager : IUserDataManager
    {
        private IRepository<Guid, User> _userRepo;

        private IRepository<Guid, Login> _loginRepo;

        private UserDataGetter _userDataGetter;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private UsersGrpcClient _loginInformationGetter;
        public UserDataManager(IRepository<Guid, User> userRepo,
            IRepository<Guid, Login> loginRepo,
            UserDataGetter userDataGetter,
            CurrentUserCredentialsGetter credentialsGetter,
            UsersGrpcClient loginInformationGetter)
        {
            _userRepo = userRepo;
            _loginRepo = loginRepo;
            _userDataGetter = userDataGetter;
            _credentialsGetter = credentialsGetter;
            _loginInformationGetter = loginInformationGetter;
        }

        public virtual async Task<(Login, User)> AddUserIfNotExisted(string tokenId, string originUserId, string accessToken)
        {

            // if The user existed but the login does not exist
            var login = _loginRepo.Table.Include(login => login.User).SingleOrDefault(t => t.TokenId == tokenId);
            if (login is null)
            {
                // add the login 
                var loginInformation = await _loginInformationGetter.GetLoginInformation(tokenId);

                var theUser = _userRepo.Table.SingleOrDefault(u => u.OriginalUserId == loginInformation.UserId);
                if (theUser is null)
                {
                    var userToBeAdded = new User
                    {
                        UserId = Guid.NewGuid(),
                        OriginalUserId = loginInformation.UserId,
                        Status = UserStatus.Available,
                        UserName = theUser.UserName,
                        Logins = new List<Login>
                        {
                            new Login
                            {
                                LoggedAt = loginInformation.LoggedAtUtc,
                                LoginId = Guid.NewGuid(),
                                TokenId = loginInformation.TokenId,
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
                        LoggedAt = loginInformation.LoggedAtUtc,
                        LoginId = Guid.NewGuid(),
                        TokenId = loginInformation.TokenId,
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
            var credentials = _credentialsGetter.GetCuurentUser();
            if (credentials is null)
            {
                return (null, null);
            }
            else
            {
                return await AddUserIfNotExisted(credentials.TokenId, credentials.UserId, credentials.AccessToken);
            }
        }

        public async Task<User> AddUserIfNeeded(string userId)
        {
            var usersById = (from u in _userRepo.Table
                            where u.OriginalUserId == userId
                            select u).FirstOrDefault();

            if(usersById is object)
            {
                return usersById;
            }

            var user = await _loginInformationGetter.GetUser(userId);
            var userToBeAdded = new User
            {
                UserId = Guid.NewGuid(),
                OriginalUserId = userId,
                Status = UserStatus.Available,
                UserName = user.Username,
            };

            _userRepo.Add(userToBeAdded);
            await _userRepo.SaveChangesAsync();

            return userToBeAdded;
        }
    }
}
