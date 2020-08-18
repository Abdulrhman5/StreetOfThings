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

        public TransactionController(ITransactionGetter transactionGetter)
        {
            _transactionGetter = transactionGetter;
        }


        [Route("foruser")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TransactionsForUser(string userId)
        {
            var trans = await _transactionGetter.GetUserTransactions(userId);
            return Ok(trans);
        }
    }
}
