using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationTestClient
{
    [Route("Test")]
    public class TestController : ControllerBase
    {
        [Route("IsAuthorized")]
        [Authorize]
        public bool IsAuthorize()
        {
            return true;
        }
    }
}
