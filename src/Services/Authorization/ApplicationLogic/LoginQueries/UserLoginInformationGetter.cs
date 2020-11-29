using DataAccessLayer;
using Models;
using System;
using System.Linq;

namespace ApplicationLogic.LoginQueries
{
    public interface IUserLoginInformationGetter
    {
        (double? longitude, double? latitude) GetUserLocation(string userId);
        LoginInformationDto GetUserLoginInformation(string tokenId);
    }

    public class LoginInformationDto
    {
        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public DateTime LoggedAtUtc { get; set; }

        public string TokenId { get; set; }
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

        public LoginInformationDto GetUserLoginInformation(string tokenId)
        {
            if (tokenId is null)
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            if (!Guid.TryParse(tokenId, out var guidTokenId))
            {
                throw new ArgumentException("Couldn't parse tokenId into Guid", nameof(tokenId));
            }

            var login = (from l in _loginRepository.Table
                         where l.LoginId == guidTokenId
                         orderby l.LoggedAt
                         descending
                         select new LoginInformationDto
                         {
                             UserId = l.UserId,
                             Username = l.User.UserName,
                             Email = l.User.Email,
                             LoggedAtUtc = l.LoggedAt,
                             TokenId = l.LoginId.ToString(),
                             Longitude = l.LoginLocation == null ? null as double? : l.LoginLocation.X,
                             Latitude = l.LoginLocation == null ? null as double? : l.LoginLocation.Y
                         }).FirstOrDefault();

            return login;
        }
    }
}
