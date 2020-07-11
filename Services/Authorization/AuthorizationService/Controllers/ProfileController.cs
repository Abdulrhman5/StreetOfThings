using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.AppUserQueries;
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

        private IUserGetter _userGetter;

        public ProfileController(ProfilePhotoSaver photoSaver, IUserGetter userGetter)
        {
            _photoSaver = photoSaver;
            _userGetter = userGetter;
        }

        [HttpPost]
        [Route("uploadPhoto")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto([FromForm]IFormFile file)
        {
            var result = await _photoSaver.SaveImage(file);
            return StatusCode(result, new
            {
                Message = "The image has been uploaded"
            });
        }


        [HttpGet]
        [Route("UserInfo")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var users = _userGetter.GetUserByIds(new List<string> { userId });
            if (users == null || !users.Any())
            {
                return NotFound();
            }
            else
            {
                return Ok(users.FirstOrDefault());
            }
        }
    }
}
