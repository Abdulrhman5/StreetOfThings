using CommonLibrary;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserCommands
{
    public interface IUserRegisterer
    {
        Task<CommandResult<Models.AppUser>> RegisterAsync(RegisterUserDto user);
    }

    public class RegisterUser : IUserRegisterer
    {
        private IRegisterUserValidator _userValidator;

        private UserManager<Models.AppUser> _userManager;

        public RegisterUser(IRegisterUserValidator userValidator, UserManager<Models.AppUser> userManager)
        {
            _userValidator = userValidator;
            _userManager = userManager;
        }

        public async Task<CommandResult<Models.AppUser>> RegisterAsync(RegisterUserDto user)
        {
            var validationResult = _userValidator.ValidateRegistrationOfNewUser(user);

            if (!validationResult.IsSuccessful)
            {
                return validationResult;
            }

            var registrationResult = await _userManager.CreateAsync(validationResult.Result, user.Password);
            if (!registrationResult.Succeeded)
            {
                return new CommandResult<Models.AppUser>(new ErrorMessage
                {
                    ErrorCode = registrationResult.Errors.FirstOrDefault().Code,
                    Message = registrationResult.Errors.FirstOrDefault().Description,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });                
            }
            return validationResult;
        }
    }
}
