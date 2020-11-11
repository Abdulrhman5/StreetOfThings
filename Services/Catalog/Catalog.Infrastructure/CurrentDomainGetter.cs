using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace Catalog.Infrastructure
{
    public class CurrentDomainGetter
    {
        private IConfiguration _configs;

        public CurrentDomainGetter(IConfiguration configs)
        {
            _configs = configs;
        }

        internal string GetDomain8Schema()
        {
            return _configs["Services:Catalog"];
        }
    }
}