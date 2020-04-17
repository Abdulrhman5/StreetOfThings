using ApplicationLogic.AppUserCommands;
using DataAccessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthorizationService.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private IUserRegisterer _userRegisterer;

        public AccountController(IUserRegisterer userRegisterer)
        {
            _userRegisterer = userRegisterer;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
        {
            var registrationResult = await _userRegisterer.RegisterAsync(user).ConfigureAwait(false);
            if (registrationResult.IsSuccessful)
            {
                return Ok(new
                {
                    Message = "A user has been created successfuly"
                });
            }
           
            return StatusCode((int) registrationResult.Error.StatusCode, registrationResult.Error);
        }
    }
}
