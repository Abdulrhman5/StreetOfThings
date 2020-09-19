using Catalog.ApplicationLogic.ObjectCommands;
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

        private IObjectDeleter _objectDeleter;
        public ObjectAdministrationController(IObjectGetter objectGetter, IObjectDeleter objectDeleter)
        {
            _objectGetter = objectGetter;
            _objectDeleter = objectDeleter;
        }

        [Route("forUser")]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<IActionResult> GetObjectsForUser(string userId)
        {
            var objects = await _objectGetter.GetObjectsOwnedByUser(userId);
            return StatusCode(200, objects);
        }

        [Route("allobjects")]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAllObjects()
        {
            var objects = await _objectGetter.GetAllObjects();
            return StatusCode(200, objects);
        }

        [Route("admin/delete")]
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteObject([FromBody] DeleteObjectDto deleteObject)
        {
            var result = await _objectDeleter.AuthorizedDelete(deleteObject);
            return StatusCode(result, new
            {
                Message = "The object has been deleted."
            });
        }
    }
}
