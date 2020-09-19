using Catalog.ApplicationLogic.GrpcClients;
using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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
                                LoggedAt = loginInformation.LoggedAtUtc,
                                LoginId = Guid.Parse(loginInformation.TokenId),
                                LoginLocation = loginInformation.Longitude is null ? null : new Point(loginInformation.Longitude.Value, loginInformation.Latitude.Value){SRID =4326 }
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
                        LoginId = Guid.Parse(loginInformation.TokenId),
                        UserId = theUser.UserId,
                        LoginLocation = loginInformation.Longitude is null ? null : new Point(loginInformation.Longitude.Value, loginInformation.Latitude.Value) { SRID = 4326}
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
    }
}
