using Grpc.Core;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileApiGateway.Services.TransactionServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway.Controllers
{
    [Route("api/Transaction/Registrations")]
    [Authorize]
    public class TransactionController : MyControllerBase
    {
        private TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Route("MeAsRecipient")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersRegistrationsOnOtherUsersObjects()
        {
            var result = await _transactionService.GetTransactionsWhereUserIsRecipient();
            return StatusCode(result);
        }

        [Route("MeAsOwner")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserObjectsTransactions()
        {
            var result = await _transactionService.GetTransactionsWhereUserIsOwner();
            return StatusCode(result);
        }
    }
}
