using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.ProfilePhotoCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : MyControllerBase
    {
        private ProfilePhotoSaver _photoSaver;

        public ProfileController(ProfilePhotoSaver photoSaver)
        {
            _photoSaver = photoSaver;
        }

        [HttpPost]
        [Route("uploadPhoto")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto([FromForm]IFormFile file)
        {
            await _photoSaver.SaveImage(file);
            return Ok();
        }
    }
}
