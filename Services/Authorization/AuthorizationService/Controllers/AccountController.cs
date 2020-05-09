using ApplicationLogic.AppUserCommands;
using ApplicationLogic.UserConfirmations;
using CommonLibrary;
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
    public class AccountController : MyControllerBase
    {
        private IUserRegisterer _userRegisterer;

        private IEmailConfirmationManager _emailConfirmationManager;
        public AccountController(IUserRegisterer userRegisterer, IEmailConfirmationManager emailConfirmationManager)
        {
            _userRegisterer = userRegisterer;
            _emailConfirmationManager = emailConfirmationManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
        {
            var registrationResult = await _userRegisterer.RegisterAsync(user).ConfigureAwait(false);
            if (registrationResult.IsSuccessful)
            {
                var confirmationCode = await _emailConfirmationManager.GenerateConfirmationCodeAsync(registrationResult.Result);
                return Ok(new
                {
                    Message = "A user has been created successfuly"
                });
            }

            return StatusCode((int) registrationResult.Error.StatusCode, registrationResult.Error);
        }

        [HttpGet]
        [Route("confirm")]
        public async Task<IActionResult> Confirm(string email, string code)
        {
            var confirmationResult =await _emailConfirmationManager.ConfirmEmailAsync(email, code);
            return StatusCode(confirmationResult, new
            {
                Message = "The email has been confirmed"
            });
        }
    }
}
