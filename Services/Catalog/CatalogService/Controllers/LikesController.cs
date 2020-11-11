using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Interfaces;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    public class LikesController : MyControllerBase
    {
        private IObjectLikeService _likeService;

        public LikesController(IObjectLikeService likeService)
        {
            _likeService = likeService;
        }

        [Route("Like")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddLike([FromBody]AddLikeDto addLikeDto)
        {
            var result = await _likeService.AddLike(addLikeDto);
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
            var result = await _likeService.Unlike(removeLikeDto);
            return StatusCode(result, new
            {
                Message = "Like has been removed."
            });
        }
    }
}
