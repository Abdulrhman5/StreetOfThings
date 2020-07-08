using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BusinessLogic.RegistrationCommands;

namespace Transaction.Service.Controllers
{
    [Route("api/Transaction/registration")]
    public class RegistrationsController : MyControllerBase
    {
        private INewRegistrationAdder _registrationAdder;

        public RegistrationsController(INewRegistrationAdder registrationAdder)
        {
            _registrationAdder = registrationAdder;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddNewRegistrationDto addnewRegistrationDto)
        {
            var result = await _registrationAdder.AddNewRegistrationAsync(addnewRegistrationDto);
            return StatusCode(result);
        }
    }
}
