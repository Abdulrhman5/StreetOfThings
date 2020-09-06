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

        public async Task<List<int>> GetUsersCountOverMonth()
        {
            var startDate = DateTime.UtcNow.AddDays(-31);
            var endDate = DateTime.UtcNow;

            var users = (from u in _usersRepo.Table.Where(uu => uu.CreatedAt >= startDate && uu.CreatedAt <= endDate)
                         group u by u.CreatedAt.Date into g
                         orderby g.Key
                         select new
                         {
                             Count = g.Count(),
                             Day = g.Key
                         }).ToList();
            var days = Enumerable.Range(0, 31).Select(offset => endDate.AddDays(-offset)).ToList();

            days.ForEach(day =>
            {
                if (!users.Any(u => u.Day.Date == day.Date))
                {
                    users.Add(new
                    {
                        Count = 0,
                        Day = day.Date
                    });
                }
            });

            var stats = users.OrderBy(u => u.Day.Date).Select(s => s.Count).ToList();

            return stats;
        }

        public async Task<List<int>> GetUsersCountOverToday()
        {
            var startDate = DateTime.UtcNow.AddHours(-24);
            var endDate = DateTime.UtcNow;

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

            var hours = Enumerable.Range(0, 24).Select(offset => {

                var dateTime = endDate.AddHours(-offset);
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
            }).ToList();

            hours.ForEach(hour =>
            {
                if (!usersHourlyFormated.Any(u => u.DateTime == hour))
                {
                    usersHourlyFormated.Add(new 
                    {
                        Count = 0,
                        DateTime = new DateTime(hour.Year, hour.Month, hour.Day, hour.Hour, 0, 0)
                    });
                }
            });
            usersHourlyFormated = usersHourlyFormated.OrderBy(u => u.DateTime).ToList();

            var stats = usersHourlyFormated.Select(u => u.Count).ToList();
            return stats;
        }
    }
}
