using System.Threading.Tasks;
using CommonLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Transaction.Core.Queries;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("foruser")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TransactionsForUser(PagingArguments pagingArguments)
        {
            var registrationsQuery = new RegisterationForUserQuery
            {
                RegistrationType = RegistrationForUserType.All,
                CurrentPage = pagingArguments.CurrentPage,
                Size = pagingArguments.Size,
                StartObject = pagingArguments.StartObject,
                Total = pagingArguments.Total
            };

            var result = await _mediator.Send(registrationsQuery);
            return Ok(result);
        }

        [Route("allTranses")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AllTransactions()
        {
            var allRegistrationQuery = new AllRegistrationQuery();

            var result = await _mediator.Send(allRegistrationQuery);
            return Ok(result);

        }

        [Route("stats/today")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsToday()
        {
            var statsQuery = new TransactionStatsOverTodayQuery();
            var result = await _mediator.Send(statsQuery);
            return Ok(result);
        }

        [Route("stats/lastMonth")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsOverMonth()
        {
            var statsQuery = new TransactionStatsOverMonthQuery();
            var result = await _mediator.Send(statsQuery);
            return Ok(result);
        }

        [Route("stats/lastYear")]
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> StatsYear()
        {
            var statsQuery = new TransactionStatsOverYearQuery();
            var result = await _mediator.Send(statsQuery);
            return Ok(result);
        }
    }
}
