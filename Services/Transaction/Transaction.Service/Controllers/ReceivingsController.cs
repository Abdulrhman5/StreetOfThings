using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Transaction.BusinessLogic.ReceivingCommands;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReceivingsController : MyControllerBase
    {
        private IObjectReceivingAdder _receivingAdder;

        public ReceivingsController(IObjectReceivingAdder receivingAdder)
        {
            _receivingAdder = receivingAdder;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddReceivingDto receivingDto)
        {
            var result = await _receivingAdder.AddReceiving(receivingDto);
            return StatusCode(result, new
            {
                Message = "The receiving has been added now you can hand over the thing.",
            });
        }
    }
}
