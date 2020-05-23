using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Catalog.ApplicationLogic.Infrastructure
{
    class CurrentUserCredentialsGetter
    {
        HttpContext _httpContext;
        public CurrentUserCredentialsGetter(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        public LoginData GetCuurentUser()
        {
            return new LoginData
            {
                AccessToken = _httpContext.Request.Headers["Authorization"],
                TokenId = _httpContext.User.FindFirst("TokenId").Value,
                UserId = _httpContext.User.FindFirst("sub").Value
            };
        }
    }

    public class LoginData
    {
        public string TokenId { get; set; }

        public string UserId { get; set; }

        public string AccessToken { get; set; }
    }
}
