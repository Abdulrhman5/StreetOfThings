using Catalog.ApplicationLogic;
using Catalog.ApplicationLogic.ObjectCommands;
using Catalog.ApplicationLogic.ObjectQueries;
using Catalog.Models;
using CommonLibrary;
using HostingHelpers;
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

        private ObjectGetter _objectGetter;

        private IObjectDeleter _objectDeleter;

        private IObjectDetailsGetter _objectDetailsGetter;
        public ObjectController(IObjectAdder objectAdder,
            PhotoAdder photoAdder,
            ObjectGetter objectGetter,
            IObjectDeleter objectDeleter, 
            IObjectDetailsGetter objectDetaissGetter)
        {
            _objectAdder = objectAdder;
            _photoAdder = photoAdder;
            _objectGetter = objectGetter;
            _objectDeleter = objectDeleter;
            _objectDetailsGetter = objectDetaissGetter;
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

        [Route("list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetObjects(PagingArguments pagings)
        {
            var objects = await _objectGetter.GetObjects(pagings);
            return StatusCode(200, objects);
        } 
        
        [Route("v1.1/list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetObjectsV1_1(PagingArguments pagings)
        {
            var objects = await _objectGetter.GetObjectsV1_1(pagings);
            return StatusCode(200, objects);
        }

        [Route("byId/{objectId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetObjects(int objectId)
        {
            var objects = await _objectGetter.GetObjectById(objectId);
            return StatusCode(200, objects);
        }


        [Route("delete")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteObject([FromBody]DeleteObjectDto objectDto)
        {
            var result = await _objectDeleter.DeleteObject(objectDto);
            return StatusCode(result, new
            {
                Message = "The object has been deleted."
            });
        }

        [Route("details")]
        public async Task<IActionResult> GetObjectDetails(int objectId)
        {
            var result = await _objectDetailsGetter.GetObjectDetails(objectId);
            return StatusCode(result);
        }
    }
}
