using ApplicationLogic.AppUserCommands;
using IdentityServer4;
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
        public const string TokenIdKey = "tokenId";
        public const String NormalizedName = "NormalizedName";
        private UserLoginManager _loginManager;
        private UserManager<AppUser> _userManager;

        public CustomProfileService(UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            UserLoginManager loginManager): base(userManager, claimsFactory)
        {
            _loginManager = loginManager;
            _userManager = userManager;
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
            var list = context.RequestedClaimTypes.ToList();
            list.Add(TokenIdKey);
            list.Add(NormalizedName);
            context.RequestedClaimTypes = list;

            if (context.Caller == IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
            {
                context.AddRequestedClaims(context.Subject.Claims.Where(c => c.Type == TokenIdKey));

                var userId = context.Subject?.GetSubjectId();  
                var user = await base.UserManager.FindByIdAsync(userId);
                var currentName = user.NormalizedName;
                var normNameClaim = new Claim(NormalizedName, currentName);
                context.AddRequestedClaims(new []{ normNameClaim });
                await base.GetProfileDataAsync(context);
            }
            else
            {
                await base.GetProfileDataAsync(context);

                var userId = context.Subject?.GetSubjectId();
                var tokenId = _loginManager.GetUserLoginId(userId);
                context.IssuedClaims.Add(new Claim(TokenIdKey, tokenId.ToString(),"tokenId"));
            }
        }
    }
}