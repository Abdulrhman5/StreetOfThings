using CommonLibrary;
using Microsoft.AspNetCore.Identity;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserCommands
{
    public interface IEmailConfirmationManager
    {
        public Task<CommandResult> ConfirmEmailAsync(string email, string confirmationCode);

        public Task<string> GenerateConfirmationCodeAsync(string userId);

        public Task<string> GenerateConfirmationCodeAsync(AppUser usre);
    }

    class EmailConfirmation : IEmailConfirmationManager
    {
        private UserManager<AppUser> _userManager;

        public EmailConfirmation(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CommandResult> ConfirmEmailAsync(string email, string ConfirmationCode)
        {
            if(email.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.NULL",
                    Message = "Please fill the email feild",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if(ConfirmationCode.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.CODE.NULL",
                    Message = "Please fill the confirmation code",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.INCORRRECT",
                    Message = "The email you are trying to confirm is not an account",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });            
            }

            if (user.EmailConfirmed)
            {
                return new CommandResult(new ErrorMessage()
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.CONFIRMED",
                    Message = "The email you are trying to confirm is already confirmed",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            var result = await _userManager.ConfirmEmailAsync(user, ConfirmationCode);
            if (!result.Succeeded)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CONFIRMATION.EMAIL.UNKOWN",
                    Message = result.Errors.FirstOrDefault().Description,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            return null;
        }

        public async Task<string> GenerateConfirmationCodeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return null;
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return code;
        }

        public async Task<string> GenerateConfirmationCodeAsync(AppUser user)
        {
            if (user == null) return null;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return code;
        }

    }
}
