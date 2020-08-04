using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    public interface IUserStatsGetter
    {
        Task<List<UserMonthlyCountStats>> GetUsersCountOverMonth();
    }

    public class UserMonthlyCountStats
    {
        public int Count { get; set; }

        public DateTime Day { get; set; }
    }
}
