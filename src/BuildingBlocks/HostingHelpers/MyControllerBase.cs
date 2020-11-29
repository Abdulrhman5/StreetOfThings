using CommonLibrary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HostingHelpers
{
    public class MyControllerBase : ControllerBase
    {

        protected IActionResult StatusCode<T>(CommandResult<T> result)
        {
            if (result.IsSuccessful)
            {
                return Ok(result.Result);
            }

            return StatusCode((int)result.Error.StatusCode, result.Error);
        }


        protected IActionResult StatusCode(CommandResult result, object successResult)
        {
            if (result.IsSuccessful)
            {
                return Ok(successResult);
            }

            return StatusCode((int)result.Error.StatusCode, result.Error);
        }

        protected IActionResult StatusCode<T>(CommandResult<T> result, object successResult)
        {
            if (result.IsSuccessful)
            {
                return Ok(successResult);
            }

            return StatusCode((int)result.Error.StatusCode, result.Error);
        }

        protected IActionResult StatusCode<T>(CommandResult<T> result, Func<object> successResult)
        {
            if (result.IsSuccessful)
            {
                return Ok(successResult());
            }

            return StatusCode((int)result.Error.StatusCode, result.Error);
        }

        protected IActionResult StatusCode(ErrorMessage error)
        {
            return StatusCode((int)error.StatusCode, error);
        }
    }
}
