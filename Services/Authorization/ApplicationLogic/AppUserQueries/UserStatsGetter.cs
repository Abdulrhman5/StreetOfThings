using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;

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
                if(!users.Any(u => u.Day.Date == day.Date))
                {
                    users.Add(new UserMonthlyCountStats
                    {
                        Day = day,
                        Count = 0
                    });
                }
            });

            users = users.OrderBy(u => u.Day.Date).ToList();
            
            return users;
        }
    }
}
