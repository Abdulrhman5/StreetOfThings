using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    public class ClientsConfig
    {
        public static List<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("MobileBff","Moble API"),
                new ApiResource("Catalog.Api", "Catalog api")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            { 
                new Client
                {
                    ClientId="MobileBff",
                    ClientSecrets =
                    {
                        new Secret("MobileBffSecret".Sha256()),
                    },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        "MobileBff",
                        "Catalog.Api",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowOfflineAccess = true,
                },
            };
    }
}
