using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    public interface IUserStatsGetter
    {
        Task<List<UserMonthlyCountStats>> GetUsersCountOverMonth();

        Task<List<int>> GetUsersCountOverToday();

        Task<UserYearlyCountListStats> GetUsersCountOverTwoYears();
    }

    public class UserYearlyCountListStats
    {
        public List<int> CurrentYear { get; set; }
         
        public List<int> PreviousYear { get; set; }
    }

    public class UserMonthlyCountStats
    {
        public int Count { get; set; }

        public DateTime Day { get; set; }
    }
}
