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
                    Id = Guid.Parse("cc0eb95f-52a0-4131-b8f6-b79ab5e7728f").ToString(),
                    Email = "TestUser@Auth.com",
                    UserName = "TestUser@Auth.com",
                    NormalizedName = "Test user",
                    NormalizedUserName = "Test user",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("5b9be4be-bac2-4677-8d3b-cfd9b749cde0"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        },

                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("dd6cafb3-b154-475e-a309-610f3d2d91bf").ToString(),
                    Email = "SecondUser@Street.com",
                    UserName = "SecondUser@Street.com",
                    NormalizedName = "Second user",
                    NormalizedUserName = "Second User",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("25291904-86a0-4b1d-b24b-fab3c332c59d"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0cac8").ToString(),
                    Email = "ThirdUser@Street.com",
                    UserName = "ThirdUser@Street.com",
                    NormalizedName = "Third user",
                    NormalizedUserName = "Third user",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c001").ToString(),
                    Email = "AbdulrhmanAlrifai@Street.com",
                    UserName = "AbdulrhmanAlrifai@Street.com",
                    NormalizedName = "Abdulrhman Alrifai",
                    NormalizedUserName = "Abdulrhman Alrifai",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0001"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c002").ToString(),
                    Email = "WaelAlzain@Street.com",
                    UserName = "WaelAlzain@Street.com",
                    NormalizedName = "Wael Alzain",
                    NormalizedUserName = "Wael Alzain",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0002"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c003").ToString(),
                    Email = "MohammedAlkhalil@Street.com",
                    UserName = "MohammedAlkhalil@Street.com",
                    NormalizedName = "Mohammed Alkhalil",
                    NormalizedUserName = "Mohammed Alkalil",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0003"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c004").ToString(),
                    Email = "QusaiAlhallaq@Street.com",
                    UserName = "QusaiAlhallaq@Street.com",
                    NormalizedName = "Qusai Alhallaq",
                    NormalizedUserName = "Qusai Alhallaq",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0004"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c005").ToString(),
                    Email = "SmeerAlhosni@Street.com",
                    UserName = "SmeerAlhosni@Street.com",
                    NormalizedName = "Smeer Alhosni",
                    NormalizedUserName = "Smeer Alhosni",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c006").ToString(),
                    Email = "AkramAlhassan@Street.com",
                    UserName = "AkramAlhassan@Street.com",
                    NormalizedName = "Akram Alhassan",
                    NormalizedUserName = "Akram Alhassan",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c007").ToString(),
                    Email = "AhmedKatza@Street.com",
                    UserName = "AhmedKatza@Street.com",
                    NormalizedName = "Ahmed Katza",
                    NormalizedUserName = "Ahmed Katza",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c008").ToString(),
                    Email = "AbdullahAlissa@Street.com",
                    UserName = "AbdullahAlissa@Street.com",
                    NormalizedName = "Abdullah Alissa",
                    NormalizedUserName = "Abdullah Alissa",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
                },
                new AppUser
                {
                    Id = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c009").ToString(),
                    Email = "MahmmodAlabd@Street.com",
                    UserName = "MahmmodAlabd@Street.com",
                    NormalizedName = "Mahmmod Alabd",
                    NormalizedUserName = "Mahmmod Alabd",
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Gender = Gender.Male,
                    Logins = new List<Login>
                    {
                        new Login
                        {
                            IsValid = true,
                            LoggedAt = DateTime.UtcNow,
                            LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                            Token = "This is dummy token, this token is generated during seeding data.",
                        }
                    }
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
