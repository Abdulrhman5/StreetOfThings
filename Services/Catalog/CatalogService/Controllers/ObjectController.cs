using Catalog.ApplicationLogic.ObjectCommands;
using Catalog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/Object")]
    public class ObjectController : MyControllerBase
    {
        private IObjectAdder _objectAdder;

        public ObjectController(IObjectAdder objectAdder)
        {
            _objectAdder = objectAdder;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddObjectDto objectDto)
        {
            var result = await _objectAdder.AddObject(objectDto);
            return StatusCode(result, new
            {
                Message = "The object has been added"
            });
        }
    }
}
