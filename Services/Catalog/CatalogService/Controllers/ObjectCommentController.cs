using Catalog.ApplicationLogic.CommentsCommands;
using Catalog.ApplicationLogic.CommentsQueries;
using CommonLibrary;
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

        private ICommentsGetter _commentsGetter;

        private IObjectCommentDeleter _commentDeleter;
        public ObjectCommentController(IObjectCommentAdder commentAdder, ICommentsGetter commentsGetter, IObjectCommentDeleter commentDeleter)
        {
            _commentAdder = commentAdder;
            _commentsGetter = commentsGetter;
            _commentDeleter = commentDeleter;
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

        [Route("forObject")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetForObject(int objectId, PagingArguments pagingArguments = null)
        {
            if (pagingArguments is null)
            {
                var result = await _commentsGetter.GetCommentsForObject(objectId);
                return Ok(result);
            }
            else
            {
                var result = await _commentsGetter.GetCommentsForObject(objectId, pagingArguments);
                return Ok(result);
            }
        }

        [Route("admin/delete")]
        [Authorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteObjectComment([FromBody] DeleteCommentDto deleteCommentDto)
        {
            var result = await _commentDeleter.AuthorizedDeleteComment(deleteCommentDto);
            return StatusCode(result, new
            {
                Message = "The comment has been deleted"
            });
        }
    }
}
