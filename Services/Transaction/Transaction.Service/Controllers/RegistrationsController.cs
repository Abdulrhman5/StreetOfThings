using CommonLibrary;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.RegistrationCommands;
using Transaction.BusinessLogic.RegistrationQueries;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationsController : MyControllerBase
    {
        private INewRegistrationAdder _registrationAdder;

        private RegistrationTokenRefresher _tokenRefresher;

        private ObjectRegistrationCanceller _registrationCanceler;

        private ITransactionGetter _transactionsGetter;

        public RegistrationsController(INewRegistrationAdder registrationAdder,
            RegistrationTokenRefresher tokenRefresher,
            ObjectRegistrationCanceller registrationCanceler, ITransactionGetter transactionsGetter)
        {
            _registrationAdder = registrationAdder;
            _tokenRefresher = tokenRefresher;
            _registrationCanceler = registrationCanceler;
            _transactionsGetter = transactionsGetter;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddNewRegistrationDto addnewRegistrationDto)
        {
            var result = await _registrationAdder.AddNewRegistrationAsync(addnewRegistrationDto);
            return StatusCode(result);
        }

        [Route("refresh")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRegistrationTokenDto tokenRefresh)
        {
            var result = await _tokenRefresher.RefreshToken(tokenRefresh.ObjectId);
            return StatusCode(result);
        }

        [Route("cancel")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelRegistration([FromBody] CancelRegistrationTokenDto cancelTokenDto)
        {
            var result = await _registrationCanceler.CancelRegistration(cancelTokenDto);
            return StatusCode(result, new
            {
                Message = "A registration has been canceled"
            });
        }

        [Route("MeAsRecipient")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersRegistrationsOnOtherUsersObjects([FromQuery] PagingArguments pagingArguments)
        {
            var result = await _transactionsGetter.GetTransactionsWhereUserIsRecipient(pagingArguments);
            return Ok(result);
        }

        [Route("MeAsOwner")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserObjectsTransactions([FromQuery] PagingArguments pagingArguments)
        {
            var result = await _transactionsGetter.GetTransactionsWhereUserIsOwner(pagingArguments);
            return Ok(result);
        }
    }
}
