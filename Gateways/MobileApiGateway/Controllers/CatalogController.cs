using CommonLibrary;
using HostingHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway.Controllers
{
    [Route("api/catalog")]
    public class CatalogController : MyControllerBase
    {
        private Services.CatalogService _catalogService;
        public CatalogController(Services.CatalogService catalogService)
        {
            _catalogService = catalogService;
            
        }

        [Route("object/list")]
        [HttpGet]
        public async Task<IActionResult> GetObjects()
        {
            return StatusCode(await _catalogService.AggregateObjects());
        }
    }
}
