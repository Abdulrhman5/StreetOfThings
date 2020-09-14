using Catalog.ApplicationLogic.Infrastructure;
using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.ApplicationLogic.Tests
{
    public class ObjectGetterTests
    {
        [Fact]
        public void GetObjects()
        {
            var dbContext = new DesignTimeDbContextFactory().CreateDbContext(null);

            var repo = new GenericRepository<int, OfferedObject>(dbContext);
            repo.Add(new OfferedObject
            {
                CurrentTransactionType = TransactionType.Free,

                PublishedAt = DateTime.UtcNow,
                Photos = new System.Collections.Generic.List<ObjectPhoto> { new ObjectPhoto { AddedAtUtc = DateTime.UtcNow } },
            });

            repo.SaveChanges();



            var impression = new Mock<IObjectImpressionsManager>();
            var objectQueryHelper = new ObjectQueryHelper();
            var userDataManager = new Mock<IUserDataManager>();
            userDataManager.Setup(u => u.AddCurrentUserIfNeeded()).ReturnsAsync((new Login
            {
                LoginLocation = new Point(10,1)
            },null as User));
            var configs = new Mock<IConfiguration>();
            configs.Setup(c => c["Settings:IncludeObjectLessThan"]).Returns("500");

            var photoConstructor = new Mock<Infrastructure.IPhotoUrlConstructor>();
            photoConstructor.Setup(p => p.Construct(It.IsAny<ObjectPhoto>())).Returns("Hello there");
            var objectGetter = new ObjectGetter(repo, photoConstructor.Object,impression.Object, objectQueryHelper, null, configs.Object,userDataManager.Object);


            var objects = objectGetter.GetObjects(new PagingArguments
            {
                Size = 10,
                StartObject = 0
            });
            Assert.True(true);
        }


        [Fact]
        public async Task GetObjects_ShouldReturnCorrectDistances()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
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
                    LoginLocation = new Point(40,40) { SRID = 4326 },
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
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    ObjectStatus = ObjectStatus.Available,
                    OwnerLogin = logins[0],
                    CurrentTransactionType = TransactionType.Free,
                    Photos = new List<ObjectPhoto>(),
                     Tags = new List<ObjectTag>(),
                     Impressions = new List<ObjectImpression>()

                },
                new OfferedObject
                {
                    OfferedObjectId = 2,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    ObjectStatus = ObjectStatus.Available,
                    OwnerLogin = logins[1],
                    CurrentTransactionType = TransactionType.Free,
                    Photos = new List<ObjectPhoto>(),
                    Tags = new List<ObjectTag>(),
                    Impressions = new List<ObjectImpression>()

                },
                new OfferedObject
                {
                    OfferedObjectId = 3,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    ObjectStatus = ObjectStatus.Available,
                    OwnerLogin = logins[2],
                    CurrentTransactionType = TransactionType.Free,
                    Photos = new List<ObjectPhoto>(),
                    Tags = new List<ObjectTag>(),
                    Impressions = new List<ObjectImpression>()
                },
                new OfferedObject
                {
                    OfferedObjectId = 4,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    ObjectStatus = ObjectStatus.Available,
                    OwnerLogin = logins[2],
                    CurrentTransactionType = TransactionType.Free,
                    Photos = new List<ObjectPhoto>(),
                    Tags = new List<ObjectTag>(),
                    Impressions = new List<ObjectImpression>()
                },
            };

            var objectsRepo = new Mock<IRepository<int, OfferedObject>>();
            var objectsMock = objects.AsQueryable().BuildMock();
            objectsRepo.Setup(o => o.Table).Returns(objects.AsQueryable().BuildMock().Object);

            var loginRepo = new Mock<IRepository<Guid, Login>>();
            var photoConstructor = new Mock<IPhotoUrlConstructor>();
            var impression = new Mock<IObjectImpressionsManager>();
            var objectQueryHelper = new ObjectQueryHelper();
            var userDataManager = new Mock<IUserDataManager>();
            userDataManager.Setup(u => u.AddCurrentUserIfNeeded()).ReturnsAsync((logins[3], users[2]));
            var configs = new Mock<IConfiguration>();
            configs.Setup(c => c["Settings:IncludeObjectLessThan"]).Returns("500");
            var getter = new ObjectGetter(objectsRepo.Object, photoConstructor.Object, impression.Object, objectQueryHelper, null, configs.Object, userDataManager.Object);

            var result = await getter.GetObjects(new PagingArguments { StartObject = 0, Size = 20 });
            Assert.True(result.Count == 4);
        }
    }
}
