using DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;

namespace AuthorizationService
{
    public class ConfigDatabases
    {

        public static void SeedUsersDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<AuthorizationContext>();
                databaseContext.Database.Migrate();

                var manager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                foreach (var user in Users)
                {
                    if (manager.FindByNameAsync(user.UserName).GetAwaiter().GetResult() is null)
                    {
                        manager.CreateAsync(user, "1qa2ws#ED").GetAwaiter().GetResult();
                    }
                }

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                foreach (var role in Roles)
                {
                    if(roleManager.FindByNameAsync(role.Name).GetAwaiter().GetResult() is null)
                    {
                        roleManager.CreateAsync(role).GetAwaiter().GetResult();
                    }
                }

                foreach(var userRole in UserRoles)
                {
                    var user = manager.FindByEmailAsync(userRole.Item1.Email).GetAwaiter().GetResult();
                    if(!manager.IsInRoleAsync(user, userRole.Role.Name).GetAwaiter().GetResult())
                    {
                        manager.AddToRoleAsync(user, userRole.Role.Name).GetAwaiter().GetResult();
                    }
                }
            }
        }

        private static List<AppUser> Users =>
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

        private static List<IdentityRole> Roles =>
            new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "Admin"
                }
            };

        private static IEnumerable<(AppUser User, IdentityRole Role)> UserRoles =>
            new List<(AppUser User, IdentityRole Role)>
            {
                (Users[0],Roles[0])
            };
    }
}
