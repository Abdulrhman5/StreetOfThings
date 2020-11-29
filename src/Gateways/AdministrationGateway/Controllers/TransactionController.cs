using AdministrationGateway.Services.TransactionServices;
using HostingHelpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdministrationGateway.Controllers
{
    [Route("api/Transaction/[controller]")]
    public class TransactionController : MyControllerBase
    {

        TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Route("foruser")]
        [HttpGet]
        public async Task<IActionResult> GetTransactionsForUser()
        {
            return StatusCode(await _transactionService.GetTransactions());
        }


        [Route("AllTranses")]
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            return StatusCode(await _transactionService.GetAllTransactions());
        }
    }
}
