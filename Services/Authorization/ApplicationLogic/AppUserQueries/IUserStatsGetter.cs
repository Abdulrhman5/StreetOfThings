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

    public class UserYearlyCountStats
    {
        public int Count { get; set; }
        public DateTime MonthYear { get; set; }
    }
    public class UserYearlyCountListStats
    {
        public List<UserYearlyCountStats> CurrentYear { get; set; }
         
        public List<UserYearlyCountStats> PreviousYear { get; set; }
    }

    public class UserMonthlyCountStats
    {
        public int Count { get; set; }

        public DateTime Day { get; set; }
    }
}
