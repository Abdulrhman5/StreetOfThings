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

        private CatalogAggregator _catalogAggregator;
        public CatalogController(Services.CatalogService catalogService, CatalogAggregator catalogAggregator)
        {
            _catalogService = catalogService;
            _catalogAggregator = catalogAggregator;
        }

        [Route("object/list")]
        [HttpGet]
        public async Task<IActionResult> GetObjects()
        {
            return StatusCode(await _catalogService.AggregateObjects());
        }    
        
        [Route("object/v1.1/list")]
        [HttpGet]
        public async Task<IActionResult> GetObjectsV1_1()
        {
            return StatusCode(await _catalogAggregator.AggregateObjectsV1_1());
        }
    }
}
