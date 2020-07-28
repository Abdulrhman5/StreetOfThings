using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingHelpers;

namespace AdministrationGateway.Controllers
{
    [Route("api/[controller]")]
    public class TestController : MyControllerBase
    {
        [Route("Dummy")]
        [Authorize]
        public IActionResult DummyData()
        {
            return Ok(new[]
            {
                new {
                    Key = 1,
                    Value = "Hello"
                },
                new
                {
                    Key = 2,
                    Value = "World"
                }
            });
        }
    }
}
