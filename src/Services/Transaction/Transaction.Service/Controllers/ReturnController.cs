using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transaction.Core.Commands;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReturnController : MyControllerBase
    {

        private IMediator _mediator;

        public ReturnController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("generate/Token")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerateReturnToken([FromBody] GenerateReturnTokenCommand command)
        {
            var result = await _mediator.Send(command);

            return StatusCode(result);
        }


        [Route("return")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReturnObject([FromBody] CreateReturningCommand command)
        {
            var result = await _mediator.Send(command);

            return StatusCode(result);
        }

    }
}
