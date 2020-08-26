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
    public class ObjectAdministration : MyControllerBase
    {
        private ObjectGetter _objectGetter;

        public ObjectAdministration(ObjectGetter objectGetter)
        {
            _objectGetter = objectGetter;
        }

        [Route("forUser")]
        [HttpGet]
        public async Task<IActionResult> GetObjects(string userId)
        {
            var objects = await _objectGetter.GetObjectsOwnerdByUser(userId);
            return StatusCode(200, objects);
        }
    }
}
