using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.Service
{
    public class ConfigDatabases
    {
        public static void SeedUsersDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<TransactionContext>();
                databaseContext.Database.Migrate();

                var transSet = databaseContext.Set<ObjectRegistration>();
                foreach(var trans in Registrations)
                {
                    if(transSet.Find(trans.Id) is null)
                    {
                        transSet.Add(trans);
                        databaseContext.SaveChanges();
                    }
                }
            }
        }


        private static List<ObjectRegistration> Registrations =>
            new List<ObjectRegistration>
            {
                new ObjectRegistration
                {
                    ObjectRegistrationId = Guid.Parse("e2c082a9-e843-4848-add5-42cbf688afcf"),
                    Status = ObjectRegistrationStatus.OK,
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(365),
                    Object = new OfferedObject
                    {
                        OriginalObjectId = 1,
                        ShouldReturn = false,
                        OwnerUser = new User
                        {
                            OriginalUserId = "cc0eb95f-52a0-4131-b8f6-b79ab5e7728f",
                            UserId = Guid.NewGuid(),
                            Status = UserStatus.Available,
                            UserName = "TestUser@Auth.com",
                            Logins = new List<Login>
                            {
                                new Login
                                {
                                    TokenId = "5b9be4be-bac2-4677-8d3b-cfd9b749cde0",
                                    LoginId = Guid.Parse("2e896d29-d338-454d-8701-dd55fd7b9d8c"),
                                },
                            }
                        },
                    },
                    RecipientLogin = new Login
                    {
                        TokenId = "25291904-86a0-4b1d-b24b-fab3c332c59d",
                        LoginId = Guid.Parse("4b7cfdfe-2f34-4b76-8064-acc6b4e53371"),
                        User = new User
                        {
                            UserId = Guid.Parse("5205b056-85ed-444f-8485-c3135b7dc3b8"),
                            UserName = "SecondUser@Street.com",
                            Status = UserStatus.Available,
                            OriginalUserId = "dd6cafb3-b154-475e-a309-610f3d2d91bf"
                        }
                    },
                    RegisteredAtUtc = DateTime.UtcNow,
                    ObjectReceiving = new ObjectReceiving
                    {
                        RecipientLoginId = Guid.Parse("4b7cfdfe-2f34-4b76-8064-acc6b4e53371"),
                        GiverLoginId = Guid.Parse("2e896d29-d338-454d-8701-dd55fd7b9d8c"),
                        ReceivedAtUtc = DateTime.UtcNow,
                        ObjectReturning = new ObjectReturning
                        {
                            LoanerLoginId =  Guid.Parse("2e896d29-d338-454d-8701-dd55fd7b9d8c"),
                            LoaneeLoginId = Guid.Parse("4b7cfdfe-2f34-4b76-8064-acc6b4e53371"),
                            ReturnedAtUtc = DateTime.UtcNow,
                        }
                    }
                },      
                
                new ObjectRegistration
                {
                    ObjectRegistrationId = Guid.Parse("41bf852e-d77e-4c8c-9ac2-cd461138a67d"),
                    Status = ObjectRegistrationStatus.OK,
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(365),    
                    Object = new OfferedObject
                    {
                        OriginalObjectId = 2,
                        ShouldReturn = false,
                        OwnerUserId = Guid.Parse("5205b056-85ed-444f-8485-c3135b7dc3b8")
                    },
                    RecipientLogin = new Login
                    {
                        TokenId = "b1590daf-7004-48f0-8af5-bc6ba97d5bed",
                        LoginId = Guid.Parse("6459c68f-0b1b-447d-9cfc-19b13b013494"),
                        User = new User
                        {
                            UserId = Guid.NewGuid(),
                            UserName = "ThirdUser@Street.com",
                            Status = UserStatus.Available,
                            OriginalUserId = "9b4210dc-49b9-4031-9a7a-dcc769a0cac8"
                        }
                    },
                    RegisteredAtUtc = DateTime.UtcNow,
                },
            };
    }
}
