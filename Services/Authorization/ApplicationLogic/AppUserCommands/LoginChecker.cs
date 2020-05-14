using CommonLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserCommands
{
    public interface ILoginChecker
    {
        public Task<CommandResult> CheckLogin(LoginCheckDto loginDto);
    }

    class LoginChecker : ILoginChecker
    {
        private SignInManager<AppUser> _signInManager;

        private ILogger<LoginChecker> _logger;

        private UserManager<AppUser> _userManager;

        public LoginChecker(SignInManager<AppUser> signInManager, ILogger<LoginChecker> logger, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<CommandResult> CheckLogin(LoginCheckDto loginDto)
        {
            if(loginDto == null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CHECK.LOGIN.EMPTYDATA",
                    Message = "Please fill out the username, password",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            if (loginDto.Password.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CHECK.LOGIN.EMPTYPASSWORD",
                    Message = "Please fill out the password field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (loginDto.Username.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CHECK.LOGIN.EMPTYUSERNAME",
                    Message = "Please fill out the username field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            AppUser user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null)
            {
                SignInResult val = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
                if (val.Succeeded)
                {
                    string sub = await _userManager.GetUserIdAsync(user);
                    LoggerExtensions.LogInformation(_logger, "Credentials validated for username: {username}", new object[1]
                    {
                        loginDto.Username
                    });
                    return new CommandResult();
                }
                if (val.IsLockedOut)
                {
                    LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: locked out", new object[1]
                    {
                        loginDto.Username
                    });
                    return new CommandResult(new ErrorMessage
                    {
                        ErrorCode = "USER.LOGIN.LOCKEDOUT",
                        Message = "There are too many falling tries to login to your account",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }
                else if (val.IsNotAllowed)
                {
                    LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: not allowed", new object[1]
                    {
                        loginDto.Username
                    });
                    return new CommandResult(new ErrorMessage
                    {
                        ErrorCode = "USER.LOGIN.NOTALLOWED",
                        Message = "This account is not allowed to be logged in",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }
                else
                {
                    LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: invalid credentials", new object[1]
                    {
                        loginDto.Username
                    });
                    return new CommandResult(new ErrorMessage
                    {
                        ErrorCode = "USER.LOGIN.INVALID.CREDENTIALS",
                        Message = "You provided wrong credentials",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }
            }
            else
            {
                LoggerExtensions.LogInformation(_logger, "No user found matching username: {username}", new object[1]
                {
                        loginDto.Username
                });
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.LOGIN.INVALID.CREDENTIALS",
                    Message = "You provided wrong credentials",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });

            }
        }
    }
}
