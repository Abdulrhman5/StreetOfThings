using ApplicationLogic.AppUserCommands;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationService.Identity
{
    public class CustomProfileService : ProfileService<AppUser>
    {

        private UserLoginManager _loginManager;

        public CustomProfileService(UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            UserLoginManager loginManager): base(userManager, claimsFactory)
        {
            _loginManager = loginManager;   
        }

        
        public CustomProfileService(UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory, 
            ILogger<ProfileService<AppUser>> logger,
            UserLoginManager loginManager) : base(userManager, claimsFactory,logger)
        {
            _loginManager = loginManager;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);

            var userId = context.Subject?.GetSubjectId();
            var tokenId = _loginManager.GetUserLoginId(userId);
            context.IssuedClaims.Add(new Claim("tid", tokenId.ToString()));
        }
    }
}
