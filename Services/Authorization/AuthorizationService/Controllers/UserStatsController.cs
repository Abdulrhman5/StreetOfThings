using ApplicationLogic.AppUserQueries;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationService.Controllers
{
    [Route("api/Stats/Users")]
    public class UserStatsController : MyControllerBase
    {
        private IUserStatsGetter _userStats;

        public UserStatsController(IUserStatsGetter userStats)
        {
            _userStats = userStats;
        }

        [Route("lastMonth")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UsersStatsLastMonth()
        {
            var stats = await _userStats.GetUsersCountOverMonth();
            return Ok(stats);
        }

        [Route("today")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersStatsToday()
        {
            var stats = await _userStats.GetUsersCountOverToday();
            return Ok(stats);
        }
    }
}
