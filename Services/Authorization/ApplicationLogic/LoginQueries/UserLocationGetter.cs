using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationLogic.LoginQueries
{
    public interface IUserLocationGetter
    {
        (double? longitude, double? latitude) GetUserLocation(string userId);
    }

    class UserLocationGetter : IUserLocationGetter
    {
        private IRepository<Guid, Login> _loginRepository;

        public UserLocationGetter(IRepository<Guid, Login> loginRepository)
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
    }
}
