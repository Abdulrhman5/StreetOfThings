using DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService
{
    public class ConfigDatabases
    {

        public static void SeedUsersDatabase(IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<AuthorizationContext>();
                databaseContext.Database.Migrate();

                var manager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                foreach(var user in Users)
                {
                    if (manager.FindByNameAsync(user.UserName).GetAwaiter().GetResult() is null)
                    {
                        manager.CreateAsync(user, "1qa2ws#ED").GetAwaiter().GetResult();
                    }
                }
            }
        }

        private static IEnumerable<AppUser> Users =>
            new List<AppUser>
            {
                new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "TestUser@Auth.com",
                    UserName = "TestUser@Auth.com",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                }
            };
    }
}
