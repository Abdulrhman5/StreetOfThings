using Catalog.ApplicationLogic.ObjectCommands;
using Catalog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        private PhotoAdder _photoAdder;
        public ObjectController(IObjectAdder objectAdder,PhotoAdder photoAdder)
        {
            _objectAdder = objectAdder;
            _photoAdder = photoAdder;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddObjectDto objectDto)
        {
            var result = await _objectAdder.AddObject(objectDto);
            return StatusCode(result, () => new
            {
                Message = "The object has been added",
                ObjectId = result.Result.Id
            });
        }
            

        [Route("{objId:int}/uploadPhoto")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadPhotoToObject([FromForm] IFormFile file, int objId)
        {
            var result = await _photoAdder.AddPhotoToObject(objId, file);
            return StatusCode(result, new
            {
                Message = "A Photo had been uploaded",
            });
        }
    }
}
