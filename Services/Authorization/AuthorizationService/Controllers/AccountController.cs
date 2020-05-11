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

        private UserManager<AppUser> _userManager;
        public AccountController(IUserRegisterer userRegisterer,
            IEmailConfirmationManager emailConfirmationManager,
            IStringIntoFileInjector stringInjector,
            IEmailSender emailSender,
            UserManager<AppUser> userManager)
        {
            _userRegisterer = userRegisterer;
            _emailConfirmationManager = emailConfirmationManager;
            _stringInjector = stringInjector;
            _emailSender = emailSender;
            _userManager = userManager;
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
        public async Task<IActionResult> Confirm(ConfirmEmailDto confirmEmailDto)
        {
            var confirmationResult =await _emailConfirmationManager.ConfirmEmailAsync(confirmEmailDto);
            return StatusCode(confirmationResult, new
            {
                Message = "The email has been confirmed"
            });
        }


        [HttpPost]
        [Route("ResendCode")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto email)
        {
            if (email is null || email.Email.IsNullOrEmpty())
            {
                var response = new ErrorMessage
                {
                    ErrorCode = "EMAIL.CONFIRMATION.RESEND.EMAIL.EMPTY",
                    Message = "Please fill the email field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return StatusCode(response);
            }

            var user = await _userManager.FindByEmailAsync(email.Email);
            if(user is null)
            {
                var response = new ErrorMessage
                {
                    ErrorCode = "EMAIL.CONFIRMATION.RESEND.EMAIL.FAULTY",
                    Message = "The email you provided is faulty",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return StatusCode(response);
            }

            // generate a confirmation code and send it to the user's email;
            var confirmationCode = await _emailConfirmationManager.GenerateConfirmationCodeAsync(user);
            var body = await _stringInjector.GetInjectedHtmlFileAsync("EmailConfirmationTemplete.html", confirmationCode);
            var emailMessage = new Email
            {
                HtmlBody = body,
                Subject = "Confirm Your Email",
                TextBody = "Please confirm your email by using this code: " + confirmationCode
            };
            await _emailSender.SendEmailByEmailAsync(user.Email , emailMessage);

            return Ok(new
            {
                Message = $"An email was sent to {email.Email} with the confirmation code"
            });
        }
    }



}
