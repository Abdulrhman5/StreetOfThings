using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationLogic.AppUserCommands
{
    public class UserLoginManager
    {
        private IRepository<Guid, Login> _loginRepo;

        private HttpContext _httpContext;

        public UserLoginManager(IRepository<Guid, Login> loginRepo, IHttpContextAccessor accessor)
        {
            _loginRepo = loginRepo;
            _httpContext = accessor.HttpContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login">should has UserId set, </param>
        public void UserLoggedIn(Login login)
        {
            var loginId = Guid.NewGuid();

            login.LoginId = loginId;
            login.LoggedAt = DateTime.UtcNow;
            login.IPAddress = _httpContext.Connection.RemoteIpAddress.ToString();
            login.ClientAgent = _httpContext.Request.Headers["User-Agent"].ToString();

            _loginRepo.Add(login);
            _loginRepo.SaveChanges();
            return;
        }

        public Login UserLoggedIn(LoginInformationDto loginDto, string userId)
        {
            var login = new Login
            {
                LoginId = Guid.NewGuid(),
                LoginLocation = new NetTopologySuite.Geometries.Point(loginDto.Location.Latitude, loginDto.Location.Longitude) { SRID = 4326 },
                Imei = loginDto.Imei,
                LoggedAt = DateTime.UtcNow,
                IPAddress = _httpContext.Connection.RemoteIpAddress.ToString(),
                ClientAgent = _httpContext.Request.Headers["User-Agent"].ToString(),
                UserId = userId
            };

            _loginRepo.Add(login);
            _loginRepo.SaveChanges();
            return login;
        }

        public Guid GetUserLoginId(string userId)
        {
            var tokens = from login in _loginRepo.Table
                         where login.UserId == userId
                         orderby login.LoggedAt descending
                         select login.Id;
            return tokens.FirstOrDefault();
        }
    }

    public class LoginInformationDto
    {
        public string Imei { get; set; }

        public (double Latitude, double Longitude) Location { get; set; }
    }

}
