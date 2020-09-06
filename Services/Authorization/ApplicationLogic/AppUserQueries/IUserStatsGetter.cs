using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    public interface IUserStatsGetter
    {
        Task<List<int>> GetUsersCountOverMonth();

        Task<List<int>> GetUsersCountOverToday();
    }
}
