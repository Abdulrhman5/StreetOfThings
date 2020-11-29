using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    class UserStatsGetter : IUserStatsGetter
    {
        private IRepository<string, AppUser> _usersRepo;

        public UserStatsGetter(IRepository<string, AppUser> usersRepo)
        {
            _usersRepo = usersRepo;
        }

        public async Task<List<UserMonthlyCountStats>> GetUsersCountOverMonth()
        {
            var startDate = DateTime.UtcNow.AddDays(-31);
            var endDate = DateTime.UtcNow;

            var users = (from u in _usersRepo.Table.Where(uu => uu.CreatedAt >= startDate && uu.CreatedAt <= endDate)
                         group u by u.CreatedAt.Date into g
                         orderby g.Key
                         select new UserMonthlyCountStats
                         {
                             Count = g.Count(),
                             Day = g.Key
                         }).ToList();
            var days = Enumerable.Range(0, 31).Select(offset => endDate.AddDays(-offset)).ToList();

            days.ForEach(day =>
            {
                if (!users.Any(u => u.Day.Date == day.Date))
                {
                    users.Add(new UserMonthlyCountStats
                    {
                        Day = day.Date,
                        Count = 0
                    });
                }
            });

            users = users.OrderBy(u => u.Day.Date).ToList();

            return users;
        }


        public async Task<List<int>> GetUsersCountOverToday()
        {
            // This will resolve to MM/DD/YYYY 00:00:00
            var startDate = DateTime.UtcNow.Date;

            // This will resolve to MM/DD+1/YYYY 00:00:00
            var endDate = DateTime.UtcNow.Date.AddDays(1);

            var usersHourly = (from u in _usersRepo.Table
                               where u.CreatedAt >= startDate && u.CreatedAt <= endDate
                               group u by new
                               {
                                   u.CreatedAt.Date,
                                   u.CreatedAt.Hour
                               } into g
                               select new
                               {
                                   Count = g.Count(),
                                   Date = g.Key.Date,
                                   Hour = g.Key.Hour
                               }).ToList();
            var usersHourlyFormated = usersHourly.Select(u => new
            {
                Count = u.Count,
                DateTime = new DateTime(u.Date.Year, u.Date.Month, u.Date.Day, u.Hour, 0, 0)
            }).ToList();

            var hours = Enumerable.Range(0, 24).Select(offset =>
            {

                var dateTime = startDate.AddHours(offset);
                return dateTime;
            }).ToList();

            hours.ForEach(hour =>
            {
                if (!usersHourlyFormated.Any(u => u.DateTime == hour))
                {
                    usersHourlyFormated.Add(new
                    {
                        Count = 0,
                        DateTime = hour
                    });
                }
            });
            usersHourlyFormated = usersHourlyFormated.OrderBy(u => u.DateTime).ToList();

            var stats = usersHourlyFormated.Select(u => u.Count).ToList();
            return stats;
        }

        public async Task<UserYearlyCountListStats> GetUsersCountOverTwoYears()
        {
            // end Date 6/4/ 2020
            // start date 1/1/2019
            // the stats will
            // CurrentYear : 1/1/20, 1/2/20, 1/3/20, 1/4/20, 1/5/20 = 0, 1/6/20 = 0, 1/7/20 = 0, ...
            // PreviousYear: 1/1/19, 1/2/19, 1/3/19, 1/4/19, 1/5/19 = x, 1/6/20 = y, 1/7/19 = z, ...
            var startDate = DateTime.UtcNow.AddYears(-1).AddMonths(-DateTime.UtcNow.Month + 1).AddDays(-DateTime.UtcNow.Day + 1).Date;
            var endDate = DateTime.UtcNow.Date;

            var usersMonthly = (from u in _usersRepo.Table
                                where u.CreatedAt >= startDate && u.CreatedAt <= endDate
                                group u by new
                                {
                                    u.CreatedAt.Year,
                                    u.CreatedAt.Month
                                } into g
                                select new
                                {
                                    Count = g.Count(),
                                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1)
                                }).ToList();

            var months = Enumerable.Range(0, 24).Select(offset => startDate.AddMonths(offset)).ToList();

            months.ForEach(month =>
            {
                if (!usersMonthly.Any(u => u.MonthYear.Year == month.Year && u.MonthYear.Month == month.Month))
                {
                    usersMonthly.Add(new
                    {
                        Count = 0,
                        MonthYear = month,
                    });
                }
            });

            return new UserYearlyCountListStats()
            {
                CurrentYear = usersMonthly.Where(um => um.MonthYear.Year == DateTime.UtcNow.Year).OrderBy(c => c.MonthYear).Select(c => c.Count).ToList(),
                PreviousYear = usersMonthly.Where(um => um.MonthYear.Year == DateTime.UtcNow.Year - 1).OrderBy(c => c.MonthYear).Select(c => c.Count).ToList()
            };
        }
    }
}
