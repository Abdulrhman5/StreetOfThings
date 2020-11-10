extern alias CatalogInfrastructure;

using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Catalog.ApplicationCore.Services.ObjectServices;
using CatalogInfrastructure::Catalog.Infrastructure.Services;
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
        private IObjectService _objectService;

        private ObjectPhotoService _objectPhotoService;
        public ObjectController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddObjectDto objectDto)
        {
            var result = await _objectService.AddObject(objectDto);
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
            var result = await _objectPhotoService.AddPhotoToObject(objId, file);
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
            var objects = await _objectService.GetObjects(pagings);
            return StatusCode(200, objects);
        } 
        
        [Route("v1.1/list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetObjectsV1_1(PagingArguments pagings)
        {
            var objects = await _objectService.GetObjectsV1_1(pagings);
            return StatusCode(200, objects);
        }

        [Route("byId/{objectId:int}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetObjects(int objectId)
        {
            var objects = await _objectService.GetObjectById(objectId);
            return StatusCode(200, objects);
        }


        [Route("delete")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteObject([FromBody]DeleteObjectDto objectDto)
        {
            var result = await _objectService.DeleteObject(objectDto);
            return StatusCode(result, new
            {
                Message = "The object has been deleted."
            });
        }

        [Route("details")]
        [Authorize]
        public async Task<IActionResult> GetObjectDetails(int objectId)
        {
            var result = await _objectService.GetObjectDetails(objectId);
            return StatusCode(result);
        }

        [Route("ordered")]
        [Authorize]
        public async Task<IActionResult> GetObjectsOrdered(string orderType, PagingArguments pagingArguments)
        {
            OrderByType type;
            if(orderType == null)
            {
                type = OrderByType.Default;
            }

            if(!Enum.TryParse<OrderByType>(orderType,true,out type))
            {
                return BadRequest(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.LIST.ORDER.NOT.DEFINED",
                    Message = "The order type is not recognized",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var result = await _objectService.GetObjects(type, pagingArguments);
            return Ok(result);
        }
    }
}
