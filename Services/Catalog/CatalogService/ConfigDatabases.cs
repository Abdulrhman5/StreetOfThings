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
                        TokenId = "5b9be4be-bac2-4677-8d3b-cfd9b749cde0",
                        User = new User
                        {
                            OriginalUserId = "cc0eb95f-52a0-4131-b8f6-b79ab5e7728f",
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
