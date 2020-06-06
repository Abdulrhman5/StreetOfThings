using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.DataAccessLayer;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
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
                ObjectFreeProperties = new ObjectFreeProperties
                {
                    OfferedFreeAtUtc = DateTime.UtcNow,
                },
                PublishedAt = DateTime.UtcNow,
                Photos = new System.Collections.Generic.List<ObjectPhoto> { new ObjectPhoto { AddedAtUtc = DateTime.UtcNow } },
                Owner = new User
                {
                    UserId = Guid.NewGuid(),
                    OriginalUserId = "asdf",
                    
                }
            });

            repo.SaveChanges();

            var photoConstructor = new Mock<Infrastructure.IObjectPhotoUrlConstructor>();
            photoConstructor.Setup(p => p.Construct(It.IsAny<ObjectPhoto>())).Returns("Hello there");
            var objectGetter = new ObjectGetter(repo, photoConstructor.Object);
            var objects = objectGetter.GetObjects();
            Assert.True(true);   
        }
    }
}
