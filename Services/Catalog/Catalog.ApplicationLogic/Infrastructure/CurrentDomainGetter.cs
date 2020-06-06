using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public class CurrentDomainGetter
    {
        private HttpContext _httpContext;

        public CurrentDomainGetter(IHttpContextAccessor contextAccessor)
        {
            _httpContext = contextAccessor.HttpContext;
        }

        internal string GetDomain8Schema()
        {
            return new Uri(_httpContext.Request.GetDisplayUrl()).GetLeftPart(UriPartial.Authority);
        }
    }
}