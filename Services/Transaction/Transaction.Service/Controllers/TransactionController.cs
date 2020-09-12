using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transaction.BusinessLogic.RegistrationQueries;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private ITransactionGetter _transactionGetter;

        private TransactionStatisticsGetter _statsGetter;
        public TransactionController(ITransactionGetter transactionGetter, TransactionStatisticsGetter statsGetter)
        {
            _transactionGetter = transactionGetter;
            _statsGetter = statsGetter;
        }


        [Route("foruser")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TransactionsForUser(string userId)
        {
            var trans = await _transactionGetter.GetUserTransactions(userId);
            return Ok(trans);
        }

        [Route("allTranses")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllTransactions()
        {
            var trans = await _transactionGetter.GetAllTransactions();
            return Ok(trans);
        }

        [Route("stats/today")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsToday()
        {
            var stats = await _statsGetter.GetTransactionsCountOverToday();
            return Ok(stats);
        }

        [Route("stats/lastMonth")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsOverMonth()
        {
            var stats = await _statsGetter.GetTransactionsCountOverMonth();
            return Ok(stats);
        }

        [Route("stats/lastYear")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsYear()
        {
            var stats = await _statsGetter.GetTransactionsCountOverYear();
            return Ok(stats);
        }
    }
}
