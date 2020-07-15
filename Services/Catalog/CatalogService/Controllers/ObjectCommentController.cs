using Catalog.ApplicationLogic.CommentsCommands;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/object/comment")]
    public class ObjectCommentController : MyControllerBase
    {
        private IObjectCommentAdder _commentAdder;

        public ObjectCommentController(IObjectCommentAdder commentAdder)
        {
            _commentAdder = commentAdder;
        }

        [Route("add")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddCommentDto commentDto)
        {
            var result = await _commentAdder.AddComment(commentDto);
            return StatusCode(result, new
            {
                Message = "A New comment has been added"
            });
        }
    }
}
