using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using Transaction.Core.Commands;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReceivingsController : MyControllerBase
    {
        private IMediator _mediator;

        public ReceivingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReceivingCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result);
        }
    }
}
