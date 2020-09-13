using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogService
{
    public class ConfigDatabases
    {
        public static void SeedUsersDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<CatalogContext>();
                databaseContext.Database.Migrate();


                var objectSet = databaseContext.Set<OfferedObject>();
                foreach (var @object in Objects)
                {
                    if (objectSet.SingleOrDefault(o => o.Name == @object.Name) is null)
                    {
                        objectSet.Add(@object);
                        databaseContext.SaveChanges();

                    }
                }
            }
        }


        private static List<OfferedObject> Objects =>
            new List<OfferedObject>
            {
                new OfferedObject
                {
                    Name = "This is the object name",
                    Description = "Also this is the object description",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = new Tag
                            {
                                Name = "This is a tag name",
                                Description = "This is a tag description",
                            }
                        }
                    },
                    OwnerLogin = new Login
                    {
                        LoginId = Guid.NewGuid(),
                        TokenId = "b1590daf-7004-48f0-8af5-bc6ba97d5bed",
                        User = new User
                        {
                            OriginalUserId = "9b4210dc-49b9-4031-9a7a-dcc769a0cac8",
                            UserId = Guid.NewGuid(),
                            Status = UserStatus.Available,
                            UserName = "TestUser@Auth.com"
                        }
                    }
                },
                new OfferedObject
                {
                    Name = "This is also an object name",
                    Description = "Also this is the object description",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = new Tag
                            {
                                Name = "This is a tag name",
                                Description = "This is a tag description",
                            }
                        }
                    },
                    OwnerLogin = new Login
                    {
                        LoginId = Guid.NewGuid(),
                        TokenId = "25291904-86a0-4b1d-b24b-fab3c332c59d",
                        User = new User
                        {
                            OriginalUserId = "dd6cafb3-b154-475e-a309-610f3d2d91bf",
                            UserId = Guid.NewGuid(),
                            Status = UserStatus.Available,
                            UserName = "SecondUser@Street.com",
                        }
                    }
                }
            };
    }
}
