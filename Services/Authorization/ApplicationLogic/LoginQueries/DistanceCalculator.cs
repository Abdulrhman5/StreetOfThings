using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CommonLibrary;

namespace ApplicationLogic.AppUserQueries
{
    public interface IDistanceCalcultaor
    {
        Task<List<(double? distance, string userId)>> CalculateDistancesAsync(string theUser, List<string> usersIds);
    }
    class DistanceCalculator : IDistanceCalcultaor
    {
        private IRepository<string, AppUser> _usersRepo;

        private IRepository<Guid, Login> _loginsRepo;

        public DistanceCalculator(IRepository<string, AppUser> usersRepo, IRepository<Guid, Login> loginsRepo)
        {
            _usersRepo = usersRepo;
            _loginsRepo = loginsRepo;
        }

        public async Task<List<(double? distance, string userId)>> CalculateDistancesAsync(string theUserId, List<string> usersIds)
        {
            if (string.IsNullOrEmpty(theUserId))
            {
                throw new ArgumentException("TheuserId is null or empty", nameof(theUserId));
            }

            if (usersIds is null)
            {
                throw new ArgumentNullException(nameof(usersIds));
            }

            var theUserLogin = await (from l in _loginsRepo.Table
                               where l.UserId == theUserId
                               orderby l.LoggedAt
                               descending
                               select l).FirstOrDefaultAsync();

            var usersDistancies = from u in _usersRepo.Table
                        where usersIds.Any(uid => uid == u.Id)
                        let lastLogin = u.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault()
                        select new
                        {
                            Distance = lastLogin == null ? null as double? : lastLogin.LoginLocation.Distance(theUserLogin.LoginLocation),
                            UserId = u.Id,
                        };

            return (await usersDistancies.ToListAsync())
                .Select(ud => (ud.Distance, ud.UserId))
                .ToList();
        }
    }
}
