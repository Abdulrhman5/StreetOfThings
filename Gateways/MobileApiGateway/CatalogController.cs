using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileApiGateway
{
    [Route("api/catalog")]
    public class CatalogController : Controller
    {
        private CatalogService _catalogService;
        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
            
        }

        [Route("object/list")]
        [HttpGet]
        public async Task<IActionResult> GetObjects()
        {
            return await _catalogService.AggregateObjects();
        }
    }
}
