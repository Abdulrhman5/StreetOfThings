using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
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

                var userSet = databaseContext.Set<User>();
                foreach (var user in Users)
                {
                    if (userSet.SingleOrDefault(u => u.UserId == user.UserId) is null)
                    {
                        userSet.Add(user);
                        databaseContext.SaveChanges();
                    }
                }


                var loginSet = databaseContext.Set<Login>();
                foreach (var login in Logins)
                {
                    if (loginSet.SingleOrDefault(o => o.LoginId == login.LoginId) is null)
                    {
                        loginSet.Add(login);
                        databaseContext.SaveChanges();

                    }
                }


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


        private static List<User> Users => new List<User>
        {
            new User
            {
                UserId = Guid.Parse("cc0eb95f-52a0-4131-b8f6-b79ab5e7728f"),
                Status = UserStatus.Available,
                UserName = "TestUser@Auth.com",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0cac8"),
                Status = UserStatus.Available,
                UserName = "ThirdUser@Auth.com",
            },
            new User
            {
                UserId = Guid.Parse("dd6cafb3-b154-475e-a309-610f3d2d91bf"),
                Status = UserStatus.Available,
                UserName = "SecondUser@Street.com",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c001"),
                Status = UserStatus.Available,
                UserName = "Abdulrhman Alrifai",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c002"),
                Status = UserStatus.Available,
                UserName = "Malak Alzain",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c003"),
                Status = UserStatus.Available,
                UserName = "Mohammed Alkhalil",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c004"),
                Status = UserStatus.Available,
                UserName = "Ousai Alhallaq",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c005"),
                Status = UserStatus.Available,
                UserName = "Amal Alhosni",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c006"),
                Status = UserStatus.Available,
                UserName = "Akram Alhassan",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c007"),
                Status = UserStatus.Available,
                UserName = "Ahmed Katza",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c008"),
                Status = UserStatus.Available,
                UserName = "Ellen Alissa",
            },
            new User
            {
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c009"),
                Status = UserStatus.Available,
                UserName = "Mahmmod Alabd",
            },
        };


        private static List<Login> Logins => new List<Login>
        {
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("5b9be4be-bac2-4677-8d3b-cfd9b749cde0"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("cc0eb95f-52a0-4131-b8f6-b79ab5e7728f"),
                LoginLocation = new Point(34.721620, 36.716978) { SRID = 4326 }
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("25291904-86a0-4b1d-b24b-fab3c332c59d"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("dd6cafb3-b154-475e-a309-610f3d2d91bf"),
                LoginLocation = new Point(34.716170, 36.706356) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d5bed"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0cac8"),
                LoginLocation = new Point(34.713824, 36.713309) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0001"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c001"),
                LoginLocation = new Point(34.738445, 36.718879) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0002"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c002"),
                LoginLocation = new Point(34.730167, 36.704941) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0003"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c003"),
                LoginLocation = new Point(34.722637, 36.701851) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0004"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c004"),
                LoginLocation = new Point(34.726623, 36.698740) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0005"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c005"),
                LoginLocation = new Point(34.728351, 36.704598) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0006"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c006"),
                LoginLocation = new Point(34.734901, 36.709947) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0007"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c007"),
                LoginLocation = new Point(34.738260, 36.712098) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0008"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c008"),
                LoginLocation = new Point(34.736100, 36.713396) { SRID = 4326 },
            },
            new Login
            {
                LoggedAt = DateTime.UtcNow,
                LoginId = Guid.Parse("b1590daf-7004-48f0-8af5-bc6ba97d0009"),
                Token = "This is dummy token, this token is generated during seeding data.",
                UserId = Guid.Parse("9b4210dc-49b9-4031-9a7a-dcc769a0c009"),
                LoginLocation = new Point(34.735972, 36.715579) { SRID = 4326 },
            },
        };

        private static List<Tag> Tags => new List<Tag>
        {
            new Tag
            {
                Name = "Wood", //0
            },
            new Tag
            {
                Name = "Paint",
            },
            new Tag
            {
                Name = "Electricals", //2
            },
            new Tag
            {
                Name = "Clothes",
            },
            new Tag
            {
                Name = "Kitchen", //4
            },
            new Tag
            {
                Name = "Tool",
            },
            new Tag
            {
                Name = "Books", //6
            },
        };
        private static List<OfferedObject> Objects =>
            new List<OfferedObject>
            {
                new OfferedObject
                {
                    Name = "Wood Hammer",
                    Description = @" 20 oz claw hammer for a wide variety of uses, Curved claw head for prying and pulling nails and more;
                        flat hammer head delivers powerful strikes, Ideal for construction, home improvement, general repairs and 
                        maintenance, woodworking, art hanging, and more ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[0]
                        }, 
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        }
                    },  
                    OwnerLoginId = Logins[0].LoginId,   
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o1p1.jpg",
                            AdditionalInformation = "?Name=o1p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o1p2.jpg",
                            AdditionalInformation = "?Name=o1p2&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o1p3.jpg",
                            AdditionalInformation = "?Name=o1p3&Version=1",
                        },
                    },
                    Comments = new List<ObjectComment>
                    {
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Helloo there how are you?",
                               LoginId= Logins[4].LoginId,
                        },                       
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hi There?",
                               LoginId= Logins[5].LoginId,
                        },                      
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hello World",
                               LoginId= Logins[9].LoginId,
                        },                      
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hiii",
                               LoginId= Logins[8].LoginId,
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Screwdriver Multitool",
                    Description = "Screwdriver Multitool, KER 7 in 1 Multi-bit Screw Driver Kit, Industrial Strength Phillips and Slotted Tip, Professional Repair Tool",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[0],
                        },
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        }
                    },
                    OwnerLoginId = Logins[0].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o2p1.jpg",
                            AdditionalInformation = "?Name=o2p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o2p2.jpg",
                            AdditionalInformation = "?Name=o2p2&Version=1",
                        },
                    },
                    Comments = new List<ObjectComment>
                    {
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Helloo there how are you?",
                               LoginId= Logins[4].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hi There?",
                               LoginId= Logins[5].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hello World",
                               LoginId= Logins[9].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hiii",
                               LoginId= Logins[8].LoginId,
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Cordless Screwdriver",
                    Description = @"Magnetic bit holder
                        Features pivot to reach tight areas
                        Includes LED lights ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[5], 
                        },
                        new ObjectTag
                        {
                            Tag = Tags[0]
                        },
                        new ObjectTag
                        {
                            Tag = Tags [2],
                        }
                    },
                    OwnerLoginId = Logins[1].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o3p1.jpg",
                            AdditionalInformation = "?Name=o3p1&Version=1",
                        },                       
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o3p2.jpg",
                            AdditionalInformation = "?Name=o3p2&Version=1",
                        },                    
                    },
                    Comments = new List<ObjectComment>
                    {
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Helloo there how are you?",
                               LoginId= Logins[4].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hi There?",
                               LoginId= Logins[5].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hello World",
                               LoginId= Logins[9].LoginId,
                        },
                        new ObjectComment
                        {
                                AddedAtUtc = DateTime.UtcNow,
                               Comment = "Hiii",
                               LoginId= Logins[8].LoginId,
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Wood Chain Saw",
                    Description = @" ""Tool-less"" blade and chain adjustments for convenient operation and easy maintenance
                        Large trigger switch with soft start for smooth start-ups
                        Rubberized soft grip handle provides increased comfort on the job",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[0]
                        },
                        new ObjectTag
                        {
                            Tag = Tags[5],
                        },
                        new ObjectTag
                        {
                            Tag = Tags[2]
                        }
                    },
                    OwnerLoginId = Logins[1].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o4p1.jpg",
                            AdditionalInformation = "?Name=o4p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o4p2.jpg",
                            AdditionalInformation = "?Name=o4p2&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Cold Steel Axe",
                    Description = "Cold Steel All-Purpose Axe with Hickory Handle, Great for Camping, Survival, Outdoors, Wood Cutting and Splitting",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-2),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[0]
                        },
                        new ObjectTag
                        {
                            Tag = Tags[5],
                        }
                    },
                    OwnerLoginId = Logins[2].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o5p1.jpg",
                            AdditionalInformation = "?Name=o5p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o5p2.jpg",
                            AdditionalInformation = "?Name=o5p2&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o5p3.jpg",
                            AdditionalInformation = "?Name=o5p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Paint Cup",
                    Description = @" Disposable cup holds up to a pint of paint
                        Built-in magnetic brush holder
                        Perfect for trim work and craft projects
                        Holds up to a pint of paint ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-1),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        },
                        new ObjectTag
                        {
                            Tag = Tags[1]
                        }
                    },
                    OwnerLoginId = Logins[2].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o6p1.jpg",
                            AdditionalInformation = "?Name=o6p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o6p2.jpg",
                            AdditionalInformation = "?Name=o6p2&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o6p3.jpg",
                            AdditionalInformation = "?Name=o6p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Roller Frame",
                    Description = "Wooster Brush R017-9 Roller Frame, 9-Inch",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-2),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[1],
                        },
                        new ObjectTag
                        {
                            Tag= Tags[5]
                        }
                    },
                    OwnerLoginId = Logins[3].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o7p1.jpg",
                            AdditionalInformation = "?Name=o7p1&Version=1",
                        },                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o7p2.jpg",
                            AdditionalInformation = "?Name=o7p2&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Magimate Paint Brush",
                    Description = "Magimate Large Paint Brush, 8 Inch, Wide Stain Brush for Floors, Doors, Wallpaper Paste and Decks, Soft Synthetic Filament with Ergonomic Handle",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-3),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        }, 
                        new ObjectTag
                        {
                            Tag = Tags[1]
                        }
                    },
                    OwnerLoginId = Logins[3].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o8p1.jpg",
                            AdditionalInformation = "?Name=o8p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o8p2.jpg",
                            AdditionalInformation = "?Name=o8p2&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o8p3.jpg",
                            AdditionalInformation = "?Name=o8p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Paint Brush",
                    Description = @"Corona 2.5"" Excalibur Chinex Paint Brush Hand-formed chisels
                        Unfinished hardwood sash handles
                        Stainless steel ferrules ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-7),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        },
                        new ObjectTag
                        {
                            Tag = Tags[1]
                        }
                    },
                    OwnerLoginId = Logins[4].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o9p1.jpg",
                            AdditionalInformation = "?Name=o9p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o9p2.jpg",
                            AdditionalInformation = "?Name=o9p2&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o9p3.jpg",
                            AdditionalInformation = "?Name=o9p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Razor Tooth Saw",
                    Description = "Corona RS 7245 Razor Tooth Folding Saw, 7-Inch Curved Blade",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddHours(-3),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[5]
                        },
                        new ObjectTag
                        {
                            Tag = Tags[0]
                        }
                    },
                    OwnerLoginId = Logins[4].LoginId,

                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o10p1.jpg",
                            AdditionalInformation = "?Name=o10p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o10p2.jpg",
                            AdditionalInformation = "?Name=o10p2&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o10p3.jpg",
                            AdditionalInformation = "?Name=o10p3&Version=1",
                        },
                    }            
                },
                new OfferedObject
                {
                    Name = "Stainless Strainer",
                    Description = "Hiware Solid Stainless Steel Spider Strainer Skimmer Ladle for Cooking and Frying, 5.4 Inch ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[4]
                        }
                    },
                    OwnerLoginId = Logins[5].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o11p1.jpg",
                            AdditionalInformation = "?Name=o11p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o11p2.jpg",
                            AdditionalInformation = "?Name=o11p2&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o11p3.jpg",
                            AdditionalInformation = "?Name=o11p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Cast iron skillet",
                    Description = @"Lodge Pre-Seasoned Cast Iron Skillet With Assist Handle, 10.25"", Black",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-2),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[4]
                        }
                    },
                    OwnerLoginId = Logins[5].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o12p1.jpg",
                            AdditionalInformation = "?Name=o12p1&Version=1",
                        }, 
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o12p2.jpg",
                            AdditionalInformation = "?Name=o12p2&Version=1",
                        },                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o12p3.jpg",
                            AdditionalInformation = "?Name=o12p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Cook's Tools",
                    Description = "WUSTHOF Kitchen Cook's Tools, One Size, Black ",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[4]
                        }
                    },
                    OwnerLoginId = Logins[6].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o13p1.jpg",
                            AdditionalInformation = "?Name=o13p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o13p2.jpg",
                            AdditionalInformation = "?Name=o13p2&Version=1",
                        }
                    }
                },
                new OfferedObject
                {
                    Name = "Used jeans",
                    Description = "Used BKE Brayden jeans mens 29x32‏",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-15),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[3]
                        }
                    },
                    OwnerLoginId = Logins[6].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o14p1.jpg",
                            AdditionalInformation = "?Name=o14p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o14p2.jpg",
                            AdditionalInformation = "?Name=o14p2&Version=1",
                        }
                    }
                },
                new OfferedObject
                {
                    Name = "Used sweater",
                    Description = "",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-15),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[3]
                        }
                    },
                    OwnerLoginId = Logins[7].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o15p1.jpg",
                            AdditionalInformation = "?Name=o15p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o15p2.jpg",
                            AdditionalInformation = "?Name=o15p2&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o15p3.jpg",
                            AdditionalInformation = "?Name=o15p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Lightning Bolt Sweater‏",
                    Description = "",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-23),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[3]
                        }
                    },
                    OwnerLoginId = Logins[7].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o16p1.jpg",
                            AdditionalInformation = "?Name=o16p1&Version=1",
                        },
                        
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o16p3.jpg",
                            AdditionalInformation = "?Name=o16p3&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Wool Hat",
                    Description = @"",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-23),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[3]
                        }
                    },
                    OwnerLoginId = Logins[8].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow.AddDays(27),
                            FilePath = @"\Assets\Images\Profile\o17p1.jpg",
                            AdditionalInformation = "?Name=o17p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o17p2.jpg",
                            AdditionalInformation = "?Name=o17p2&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Sun Hat",
                    Description = "Also this is the object description",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-28),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[3]
                        }
                    },
                    OwnerLoginId = Logins[11].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o18p1.jpg",
                            AdditionalInformation = "?Name=o18p1&Version=1",
                        },
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o18p2.jpg",
                            AdditionalInformation = "?Name=o18p2&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "Mrs. Everything A Novel",
                    Description = @"A NEW YORK TIMES 100 NOTABLE BOOKS OF 2019 SELECTION
                        ONE OF NPR’S BEST BOOKS OF 2019
                        THE WASHINGTON POST’S 50 NOTABLE WORKS OF FICTION IN 2019
                        GOOD HOUSEKEEPING’S 50 BEST BOOKS OF 2019",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow.AddDays(-28),
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[6]
                        }
                    },
                    OwnerLoginId = Logins[9].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o19p1.jpg",
                            AdditionalInformation = "?Name=o19p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o19p2.jpg",
                            AdditionalInformation = "?Name=o19p2&Version=1",
                        },
                    }
                },
                new OfferedObject
                {
                    Name = "IT Architecture For Dummies",
                    Description = @"A solid introduction to the practices, plans, and skills required for developing a smart system architecture
                        Information architecture combines IT skills with business skills in order to align the IT structure of an organization with the mission, goals, and objectives of its business.",
                    CurrentTransactionType = TransactionType.Free,
                    ObjectStatus = ObjectStatus.Available,
                    PublishedAt = DateTime.UtcNow,
                    Tags = new List<ObjectTag>
                    {
                        new ObjectTag
                        {
                            Tag = Tags[6]
                        }
                    },
                    OwnerLoginId = Logins[10].LoginId,
                    Photos = new List<ObjectPhoto>
                    {
                        new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o20p1.jpg",
                            AdditionalInformation = "?Name=o20p1&Version=1",
                        },new ObjectPhoto
                        {
                            AddedAtUtc = DateTime.UtcNow,
                            FilePath = @"\Assets\Images\Profile\o20p2.jpg",
                            AdditionalInformation = "?Name=o20p2&Version=1",
                        }
                    }
                },
            };
    }
}
