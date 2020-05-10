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
using System.Threading.Tasks;using Infrastructure;
using Infrastructure.Emails;

namespace AuthorizationService.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : MyControllerBase
    {
        private IUserRegisterer _userRegisterer;

        private IEmailConfirmationManager _emailConfirmationManager;

        private IStringIntoFileInjector _stringInjector;

        private IEmailSender _emailSender;
        public AccountController(IUserRegisterer userRegisterer,
            IEmailConfirmationManager emailConfirmationManager,IStringIntoFileInjector stringInjector,
            IEmailSender emailSender)
        {
            _userRegisterer = userRegisterer;
            _emailConfirmationManager = emailConfirmationManager;
            _stringInjector = stringInjector;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
        {
            var registrationResult = await _userRegisterer.RegisterAsync(user).ConfigureAwait(false);
            if (registrationResult.IsSuccessful) 
            {

                // generate a confirmation code and send it to the user's email;
                var confirmationCode = await _emailConfirmationManager.GenerateConfirmationCodeAsync(registrationResult.Result);
                var body = await _stringInjector.GetInjectedHtmlFileAsync("EmailConfirmationTemplete.html", confirmationCode);
                var email = new Email
                {
                    HtmlBody = body,
                    Subject = "Confirm Your Email",
                    TextBody = "Please confirm your email by using this code: " + confirmationCode
                };
                await _emailSender.SendEmailByEmailAsync(registrationResult.Result.Email, email);

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
