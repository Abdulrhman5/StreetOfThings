using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.RegistrationQueries;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.Test
{
    [TestClass]
    public class TransactionsStatsGetterTests
    {

        [TestMethod]
        public async Task GetTransactionStatsOverToday()
        {
            var receivings = new List<ObjectReceiving>
            {
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddHours(1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddHours(1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddHours(1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddHours(23)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddHours(23)
                },
            };

            var receivingsRepo = new Mock<IRepository<Guid, ObjectReceiving>>();
            receivingsRepo.Setup(rc => rc.Table).Returns(receivings.AsQueryable());

            var statsGetter = new TransactionStatisticsGetter(null, receivingsRepo.Object);

            var result = await statsGetter.GetTransactionsCountOverToday();
            Assert.IsTrue(result.TransactionsOverToday.Count == 24 && result.TransactionsOverToday[1] == 3 && result.TransactionsOverToday[23] == 2);
        }

        [TestMethod]
        public async Task GetTransactionStatsOverMonth()
        {
            var receivings = new List<ObjectReceiving>
            {
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-30)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-30)
                },
            };

            var receivingsRepo = new Mock<IRepository<Guid, ObjectReceiving>>();
            receivingsRepo.Setup(rc => rc.Table).Returns(receivings.AsQueryable());

            var statsGetter = new TransactionStatisticsGetter(null, receivingsRepo.Object);

            var result = await statsGetter.GetTransactionsCountOverMonth();
            Assert.IsTrue(result.Count == 31 && result[0].Count == 2 && result[29].Count == 3);
        }

        [TestMethod]
        public async Task GetTransactionStatsOverYear()
        {
            var receivings = new List<ObjectReceiving>
            {
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-1)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-30)
                },
                new ObjectReceiving
                {
                    ReceivedAtUtc= DateTime.UtcNow.Date.AddDays(-30)
                },
            };

            var receivingsRepo = new Mock<IRepository<Guid, ObjectReceiving>>();
            receivingsRepo.Setup(rc => rc.Table).Returns(receivings.AsQueryable());

            var statsGetter = new TransactionStatisticsGetter(null, receivingsRepo.Object);

            var result = await statsGetter.GetTransactionsCountOverYear();
            Assert.IsTrue(result.Count == 12);
        }
    }
}
