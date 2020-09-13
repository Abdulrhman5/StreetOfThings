using CommonLibrary;
using HostingHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("Resources")]
    public class ResourceController : MyControllerBase
    {
        private IConfiguration _configs;

        private string _contentRoot;

        public ResourceController(IConfiguration configs)
        {
            _configs = configs;
            _contentRoot = _configs[WebHostDefaults.ContentRootKey];
        }

        [Route("Photo/{type}/{name}")]
        public async Task<IActionResult> Photo([FromRoute]string type, [FromRoute]string name)
        {
            if (type.EqualsIC("Tag") || type.EqualsIC("Object"))
            {
                var profilePhotoPath = $@"{_contentRoot}\Assets\Images\Profile\";
                var files = Directory.GetFiles(profilePhotoPath, $"{name}.*");
                if (!files.Any())
                {
                    return StatusCode(new ErrorMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        ErrorCode = "RESOURCE.PHOTO.NOT.FOUND",
                        Message = "The image you requested does not exists"
                    });
                }
                else
                {
                    var file = System.IO.File.OpenRead(files.FirstOrDefault());
                    return File(file, "Image/jpeg");
                }
            }
            else
            {
                return StatusCode(new ErrorMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ErrorCode = "RESOURCE.PHOTO.TYPE.NOT.FOUND",
                    Message = "The file you requested does not exists"
                });
            }
        }
    }
}
