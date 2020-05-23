using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.ApplicationLogic.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatalogService
{
    public class IdentityTestController : Controller
    {
        private IConfiguration _configs;
        public IdentityTestController(IConfiguration configs)
        {
            _configs = configs;
        }

        // GET: /<controller>/
        [Authorize]
        public async Task<IActionResult> Index()
        {
            UserDataGetter userGetter = new UserDataGetter(_configs);
            var userData = await userGetter.GetUserDataByToken(Request.Headers["Authorization"]);
            return new JsonResult(new { Result = "Hello there" });
        }
    }
}
