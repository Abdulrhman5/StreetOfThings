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
        private ILikeAdder _likeAdder;

        private ILikeDeleter _likeDeleter;
        public LikesController(ILikeAdder likeAdder, ILikeDeleter likeDeleter)
        {
            _likeAdder = likeAdder;
            _likeDeleter = likeDeleter;
        }

        [Route("Like")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddLike([FromBody]AddLikeDto addLikeDto)
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
        public async Task<IActionResult> RemoveLike([FromBody]AddLikeDto removeLikeDto)
        {
            var result = await _likeDeleter.Unlike(removeLikeDto);
            return StatusCode(result, new
            {
                Message = "Like has been removed."
            });
        }
    }
}
