using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transaction.BusinessLogic.ReturningCommands;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class ReturnController : MyControllerBase
    {

        private ReturnTokenGenerator _returnTokenGenerator;

        private IReturningAdder _returingAdder;

        public ReturnController(ReturnTokenGenerator returnTokenGenerator, IReturningAdder returingAdder)
        {
            _returnTokenGenerator = returnTokenGenerator;
            _returingAdder = returingAdder;
        }

        [Route("generate/Token")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerateReturnToken([FromBody]GenerateReturnTokenDto generateReturnTokenDto)
        {
            var result = await _returnTokenGenerator.GenerateToken(generateReturnTokenDto);
            return StatusCode(result);
        }

        [Route("return")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReturnObject([FromBody] ObjectReturningDto returnDto)
        {
            var result = await _returingAdder.AddObjectReturning(returnDto);
            return StatusCode(result);
        }

    }
}
