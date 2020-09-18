using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Catalog.ApplicationLogic.Tests
{
    public class ObjectQueryHelperTests
    {

        [Fact]
        public void ObjObjects_OrderByDate_ShouldReturnCorrectValues()
        {
            var objects = new List<OfferedObject>
            {
                new OfferedObject
                {
                    OfferedObjectId = 1,
                    PublishedAt = DateTime.UtcNow.AddDays(-5)
                },
                new OfferedObject
                {
                    OfferedObjectId = 2,
                    PublishedAt = DateTime.UtcNow.AddDays(-4)
                },
                new OfferedObject
                {
                    OfferedObjectId = 3,
                    PublishedAt = DateTime.UtcNow.AddDays(-3)
                },
                new OfferedObject
                {
                    OfferedObjectId = 4,
                    PublishedAt = DateTime.UtcNow.AddDays(-2)
                },
                new OfferedObject
                {
                    OfferedObjectId = 5,
                    PublishedAt = DateTime.UtcNow.AddDays(-6)
                },
            };

            var queryHelper = new ObjectQueryHelper();
            var result = queryHelper.OrderObject(objects.AsQueryable(), null, OrderByType.Date).ToList();

            Assert.True(result.Count == 5 &&
                result[0].OfferedObjectId == 4 &&
                result[1].OfferedObjectId == 3 &&
                result[4].OfferedObjectId == 6
                );
        }
              
        [Fact]
        public void ObjObjects_OrderByTopRated_ShouldReturnCorrectValues()
        {
            var objects = new List<OfferedObject>
            {
                new OfferedObject
                {
                    OfferedObjectId = 1,
                    Transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            Rating = 5
                        },
                        new Transaction
                        {
                            Rating = 5
                        },
                        new Transaction
                        {
                            Rating = 5
                        },
                    }
                },
                new OfferedObject
                {
                    OfferedObjectId = 2,
                    Transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            Rating = 10
                        },
                        new Transaction
                        {
                            Rating = 10
                        },
                        new Transaction
                        {
                            Rating = 10
                        },
                    }                },
                new OfferedObject
                {
                    OfferedObjectId = 3,
                    Transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            Rating = 1
                        },
                        new Transaction
                        {
                            Rating = 1
                        },
                        new Transaction
                        {
                            Rating = 1
                        },
                    }                
                },
                new OfferedObject
                {
                    OfferedObjectId = 4,
                    Transactions = new List<Transaction>{}
                },
                new OfferedObject
                {
                    OfferedObjectId = 5,
                    Transactions= new List<Transaction>()
                },
            };

            var queryHelper = new ObjectQueryHelper();
            var result = queryHelper.OrderObject(objects.AsQueryable(), null, OrderByType.TopRated).ToList();

            Assert.True(result.Count == 5 &&
                result[0].OfferedObjectId == 2 &&
                result[1].OfferedObjectId == 1 
                );
        }

        [Fact]
        public void ObjObjects_OrderByNearest_ShouldReturnCorrectValues()
        {
            var users = new List<User>
            {
                new User
                {
                    Status = UserStatus.Available,
                    UserName = "1",
                },
                new User
                {
                    Status = UserStatus.Available,
                    UserName = "2",
                },
                new User
                {
                    Status = UserStatus.Available,
                    UserName = "3",
                }
            };

            var logins = new List<Login>
            {
                new Login
                {
                    User = users[0],
                    LoggedAt = DateTime.UtcNow.AddDays(-10),
                    LoginLocation = new Point(40,40) { SRID = 4326 },
                },
                new Login
                {
                    User = users[0],
                    LoggedAt = DateTime.UtcNow.AddDays(0),
                    LoginLocation = new Point(10,10) { SRID = 4326 },
                },
                new Login
                {
                    User = users[1],
                    LoggedAt = DateTime.UtcNow.AddDays(10),
                    LoginLocation = new Point(20,20) { SRID = 4326 },
                },
                new Login
                {
                    User = users[2],
                    LoggedAt = DateTime.UtcNow.AddDays(10),
                    LoginLocation = new Point(10.003,10) { SRID = 4326 },
                }
            };

            users[0].Logins = new List<Login>
            {
                logins[0],logins[1]
            };
            users[1].Logins = new List<Login> { logins[2] };
            users[2].Logins = new List<Login> { logins[3] };
            var objects = new List<OfferedObject>
            {
                new OfferedObject
                {
                    OfferedObjectId = 1,
                    OwnerLogin = logins[0],
                },
                new OfferedObject
                {
                    OfferedObjectId = 2,
                    OwnerLogin = logins[2]
                },
                new OfferedObject
                {
                    OfferedObjectId = 3,
                    OwnerLogin = logins[3]
                },
            };

            var queryHelper = new ObjectQueryHelper();
            var result = queryHelper.OrderObject(objects.AsQueryable(), new Point(10, 10) { SRID = 4326 }, OrderByType.Nearest).ToList();

            Assert.True(result.Count == 3 &&
                result[0].OfferedObjectId == 1 &&
                result[1].OfferedObjectId == 3 &&
                result[2].OfferedObjectId == 2
                );
        }

        [Fact]
        public void ObjObjects_OrderBy_ShouldReturnCorrectValues()
        {
            var objects = new List<OfferedObject>
            {
                new OfferedObject
                {
                    OfferedObjectId = 1,
                    PublishedAt = DateTime.UtcNow.AddSeconds(-100),
                    Views = new List<ObjectView>
                    {
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                    }
                    // 17/ 100
                },
                new OfferedObject
                {
                    OfferedObjectId = 2,
                    PublishedAt = DateTime.UtcNow.AddSeconds(-20),
                    Views = new List<ObjectView>
                    {
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                    }
                    // 9/20 ~ 1/2
                },
                new OfferedObject
                {
                    OfferedObjectId = 3,
                    PublishedAt = DateTime.UtcNow,
                    Views = new List<ObjectView>
                    {
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                        new ObjectView{},
                    }

                    // 6/1
                },
            };

            var queryHelper = new ObjectQueryHelper();
            var result = queryHelper.OrderObject(objects.AsQueryable(), new Point(10, 10) { SRID = 4326 }, OrderByType.Trending).ToList();

            Assert.True(result.Count == 3 &&
                result[0].OfferedObjectId == 3 &&
                result[1].OfferedObjectId == 2 &&
                result[2].OfferedObjectId == 1
                );
        }

    }
}
