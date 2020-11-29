using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdministrationGateway.Services;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationGateway.Controllers
{
    [Route("api/catalog/object")]
    public class ObjectController : MyControllerBase
    {
        private ObjectAggregator _objectAggregator;

        public ObjectController(ObjectAggregator objectAggregator)
        {
            _objectAggregator = objectAggregator;
        }

        [Route("allObjects")]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<IActionResult> Index()
        {
            var result = await _objectAggregator.AggregateObjects();
            return StatusCode(result);
        }
    }
}
