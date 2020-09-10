using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationLogic.LoginQueries
{
    public interface IUserLoginInformationGetter
    {
        (double? longitude, double? latitude) GetUserLocation(string userId);
        (double? longitude, double? latitude) GetUserLoginInformation(string tokenId);
    }

    class UserLoginInformationGetter : IUserLoginInformationGetter
    {
        private IRepository<Guid, Login> _loginRepository;

        public UserLoginInformationGetter(IRepository<Guid, Login> loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public (double? longitude, double? latitude) GetUserLocation(string userId)
        {
            var login = (from l in _loginRepository.Table
                         where l.UserId == userId
                         orderby l.LoggedAt
                         descending
                         select l).FirstOrDefault();

            if (login?.LoginLocation is null)
            {
                return (null, null);
            }
            return (login.LoginLocation.X, login.LoginLocation.Y);
        }

        public (double? longitude, double? latitude) GetUserLoginInformation(string tokenId)
        {
            if (tokenId is null)
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            if(!Guid.TryParse(tokenId,out var guidTokenId))
            {
                throw new ArgumentException("Couldn't parse tokenId into Guid", nameof(tokenId));
            }

            var login = (from l in _loginRepository.Table
                         where l.LoginId == guidTokenId
                         orderby l.LoggedAt
                         descending
                         select l).FirstOrDefault();

            if (login?.LoginLocation is null)
            {
                return (null, null);
            }
            return (login.LoginLocation.X, login.LoginLocation.Y);
        }
    }
}
