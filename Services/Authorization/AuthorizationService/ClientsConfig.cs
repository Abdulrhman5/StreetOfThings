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
                new ApiResource("Catalog.Api", "Catalog api"),
                new ApiResource("ProfileManagement", "Profile management"),
                new ApiResource("Transaction.Api", "Transaction Api"),
                new ApiResource("Catalog.Admin", "Catalog Administration"),
                new ApiResource("Transaction.Admin", "Transaction Administration"),
                new ApiResource("AdminGateway", "Admin gateway")
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
                        "ProfileManagement",
                        "Transaction.Api",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowOfflineAccess = true,
                },
                new Client
                {
                    ClientId="AdminBff",
                    ClientSecrets =
                    {
                        new Secret("AdminBffSecret".Sha256()),
                    },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        "Catalog.Admin",
                        "ProfileManagement",
                        "Transaction.Admin",
                        "AdminGateway",
                        "Transaction.Api",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60*60*24*30
                },
            };
    }
}
