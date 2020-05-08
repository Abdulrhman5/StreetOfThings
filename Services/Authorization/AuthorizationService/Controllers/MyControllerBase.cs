using CommonLibrary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService.Controllers
{
    public class MyControllerBase : ControllerBase
    {

        protected IActionResult StatusCode(CommandResult result, object successResult)
        {
            if (result.IsSuccessful)
            {
                return Ok();
            }

            return StatusCode((int)result.Error.StatusCode, result.Error);
        }
    }
}
