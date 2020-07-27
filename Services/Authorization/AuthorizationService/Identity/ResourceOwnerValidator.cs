using ApplicationLogic.AppUserCommands;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AuthorizationService.Identity
{
    public class ResourceOwnerValidator : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<AppUser> _signInManager;

        private IEventService _events;

        private readonly UserManager<AppUser> _userManager;

        private readonly ILogger<ResourceOwnerValidator> _logger;

        private LoginInformationValidator _informationValidator;

        private UserLoginManager _loginManager;

        public ResourceOwnerValidator(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEventService events,
            ILogger<ResourceOwnerValidator> logger,
            UserLoginManager loginManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
            _informationValidator = new LoginInformationValidator();
            _loginManager = loginManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var loginInformationValidationResult = _informationValidator.ValidateLoginInfo(context);

            if (!loginInformationValidationResult.IsSuccess)
            {
                LoggerExtensions.LogInformation(_logger, "Login information is not formated correctly: {username}", new object[1]
                {
                    context.UserName
                });
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid username", interactive: false));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "LoginInformation should be formated like this lan=99.99&lat=99.99&Imei=xxxx");
                return;
            }

            string clientId = context.Request?.Client?.ClientId;
            AppUser user = await _userManager.FindByNameAsync(context.UserName);
            if (user is null)
            {
                LoggerExtensions.LogInformation(_logger, "No user found matching username: {username}", new object[1]
                {
                    context.UserName
                });
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false, clientId));
                return;
            }

            if(clientId == "AdminBff")
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    LoggerExtensions.LogInformation(_logger, "No user found matching username: {username}", new object[1]
                    {
                    context.UserName
                    });
                    await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false, clientId));
                    return;
                }
            }

            SignInResult val = await _signInManager.CheckPasswordSignInAsync(user, context.Password, lockoutOnFailure: true);
            if (val.Succeeded)
            {
                string sub = await _userManager.GetUserIdAsync(user);
                LoggerExtensions.LogInformation(_logger, "Credentials validated for username: {username}", new object[1]
                {
                    context.UserName
                });
                await _events.RaiseAsync(new UserLoginSuccessEvent(context.UserName, sub, context.UserName, interactive: false, clientId));

                var login = _loginManager.UserLoggedIn(loginInformationValidationResult.LoginInformation,
                    user.Id);

                var additionalAttrs = new Dictionary<string, object>
                    {
                        {"tokenId", login.Id.ToString() }
                    };
                context.Result = new GrantValidationResult(sub,
                    "pwd",
                    authTime: login.LoggedAt,
                    customResponse: additionalAttrs);
                return;
            }
            if (val.IsLockedOut)
            {
                LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: locked out", new object[1]
                {
                    context.UserName
                });
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "locked out", interactive: false, clientId));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            else if (val.IsNotAllowed)
            {
                LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: not allowed", new object[1]
                {
                    context.UserName
                });
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "not allowed", interactive: false, clientId));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            else
            {
                LoggerExtensions.LogInformation(_logger, "Authentication failed for username: {username}, reason: invalid credentials", new object[1]
                {
                        context.UserName
                });
                await _events.RaiseAsync(new UserLoginFailureEvent(context.UserName, "invalid credentials", interactive: false, clientId));
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
        }
    }
}
