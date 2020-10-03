using ApplicationLogic.AppUserQueries;
using DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.ApplicationLogic.Test
{
    [TestClass]
    public class UserStatsGetterTests
    {
        [TestMethod]
        public async Task UsersCountOverToday_ThereIsDescreteData_ShouldReturn24Element()
        {
            var userRepo = new Mock<IRepository<string, AppUser>>();
            var getter = new UserStatsGetter(userRepo.Object);
            var users = new List<AppUser>
            {
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddHours(-2),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddHours(-2),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddHours(-1),
                },
            };

            userRepo.Setup(u => u.Table).Returns(users.AsQueryable());

            var result = await getter.GetUsersCountOverToday();

            Assert.IsTrue(result.Count == 24);
        }
           
        [TestMethod]
        public async Task UsersCountOverMonth_ThereIsDescreteData_ShouldReturn31Element()
        {
            var userRepo = new Mock<IRepository<string, AppUser>>();
            var getter = new UserStatsGetter(userRepo.Object);
            var users = new List<AppUser>
            {
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-2),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-2),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-2),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-5),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-30),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-30),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-31),
                },
                new AppUser
                {
                    CreatedAt= DateTime.UtcNow.AddDays(-40),
                },
            };

            userRepo.Setup(u => u.Table).Returns(users.AsQueryable());

            var result = await getter.GetUsersCountOverMonth();

            Assert.IsTrue(result.Count == 31 && result[0].Count == 2 && result[25].Count == 1 && result[28].Count == 3);
        }


        [TestMethod]
        public async Task UsersCountOverTwoYears_ShouldReturnCorrectNumberOfElements()
        {
            var userRepo = new Mock<IRepository<string, AppUser>>();
            var getter = new UserStatsGetter(userRepo.Object);

            var result = await getter.GetUsersCountOverTwoYears();
            Assert.IsTrue(result.CurrentYear.Count == 12 &&
                result.PreviousYear.Count == 12 &&
                result.CurrentYear.Where((cs, i) => cs == i + 1).Count() == 12 &&  // using the fact the [0]=1/1/20, [1]=1/2/20, [2]=1/3/20
                result.PreviousYear.Where((cs, i) => cs == i + 1).Count() == 12);
        }

        [TestMethod]
        public async Task UsersCountOverTwoYears_HaveValues_ShouldReturnCorrectCounts()
        {
            var userRepo = new Mock<IRepository<string, AppUser>>();
            var getter = new UserStatsGetter(userRepo.Object);
            var users = new List<AppUser>
            {
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year, 4, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year, 4, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year-1, 4, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year, 5, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year, 6, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year, 7, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year-1, 8, 1),
                },
                new AppUser
                {
                    CreatedAt= new DateTime(DateTime.Now.Year-1, 8, 1),
                },
            };

            userRepo.Setup(u => u.Table).Returns(users.AsQueryable());

            var result = await getter.GetUsersCountOverTwoYears();

            Assert.IsTrue(result.CurrentYear[3] == 2 &&
                result.CurrentYear[4] == 1 &&
                result.CurrentYear[5] == 1 &&
                result.CurrentYear[6] == 1 &&
                result.PreviousYear[3] == 1 &&
                result.PreviousYear[7] == 2 
                );
        }
    }
}
