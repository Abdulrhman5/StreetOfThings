using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    public class ClientsConfig
    {

        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("testApi","My Api")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId="testClient",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()),
                    },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = 
                    {
                        "testApi" 
                    }
                }
            };


    }
}
