using Catalog.ApplicationLogic.ObjectQueries;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/object")]
    [Authorize("Admin")]
    public class ObjectAdministrationController : MyControllerBase
    {
        private IObjectGetter _objectGetter;

        public ObjectAdministrationController(IObjectGetter objectGetter)
        {
            _objectGetter = objectGetter;
        }

        [Route("forUser")]
        [HttpGet]
        public async Task<IActionResult> GetObjectsForUser(string userId)
        {
            var objects = await _objectGetter.GetObjectsOwnedByUser(userId);
            return StatusCode(200, objects);
        }

        [Route("allobjects")]
        [HttpGet]
        public async Task<IActionResult> GetAllObjects()
        {
            var objects = await _objectGetter.GetAllObjects();
            return StatusCode(200, objects);
        }
    }
}
