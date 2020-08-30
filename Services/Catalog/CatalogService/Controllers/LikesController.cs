using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.ApplicationLogic.LikeCommands;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    public class LikesController : MyControllerBase
    {
        private LikeAdder _likeAdder;

        private LikeDeleter _likeDeleter;
        public LikesController(LikeAdder likeAdder, LikeDeleter likeDeleter)
        {
            _likeAdder = likeAdder;
            _likeDeleter = likeDeleter;
        }

        [Route("Like")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddLike(AddLikeDto addLikeDto)
        {
            var result = await _likeAdder.AddLike(addLikeDto);
            return StatusCode(result, new
            {
                Message = "Like has been added."
            });
        }
           
        [Route("Unlike")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveLike(AddLikeDto removeLikeDto)
        {
            var result = await _likeDeleter.Unlike(removeLikeDto);
            return StatusCode(result, new
            {
                Message = "Like has been removed."
            });
        }
    }
}
