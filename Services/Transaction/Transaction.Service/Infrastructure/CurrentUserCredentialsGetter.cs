using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace Transaction.Service.Infrastructure
{
    public class CurrentUserCredentialsGetter
    {
        HttpContext _httpContext;

        public CurrentUserCredentialsGetter(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        public LoginData GetCuurentUser()
        {
            if(_httpContext.User is null)
            {
                return null;
            }

            return new LoginData
            {
                AccessToken = _httpContext.Request.Headers["Authorization"],
                TokenId = _httpContext.User.FindFirst("TokenId")?.Value,
                UserId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
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
