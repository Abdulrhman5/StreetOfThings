using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using CommonLibrary;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/object/comment")]
    public class ObjectCommentController : MyControllerBase
    {
        private ICommentService _commentService;

        private IRepository<Guid, ObjectComment> _commentsRepo;

        public ObjectCommentController(ICommentService commentService,
            IRepository<Guid, ObjectComment> commentsRepo)
        {
            _commentService = commentService;
            _commentsRepo = commentsRepo;
        }

        [Route("add")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddCommentDto commentDto)
        {
            var result = await _commentService.AddComment(commentDto);
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
            if (pagingArguments is object)
            {
                var comments = from comment in _commentsRepo.Table
                               where comment.ObjectId == objectId
                               orderby comment.AddedAtUtc descending
                               select new 
                               {
                                   UserId = comment.Login.UserId.ToString(),
                                   ObjectId = comment.ObjectId,
                                   Comment = comment.Comment,
                                   CommentedAtUtc = comment.AddedAtUtc,
                                   CommentId = comment.ObjectCommentId
                               };

                return Ok(new
                {
                    Comments = await comments.SkipTakeAsync(pagingArguments),
                    CommentsCount = comments.Count()
                });
            }
            else
            {
                var comments = from comment in _commentsRepo.Table
                               where comment.ObjectId == objectId
                               orderby comment.AddedAtUtc descending
                               select new
                               {
                                   UserId = comment.Login.UserId.ToString(),
                                   ObjectId = comment.ObjectId,
                                   Comment = comment.Comment,
                                   CommentedAtUtc = comment.AddedAtUtc,
                                   CommentId = comment.ObjectCommentId
                               };

                return Ok(new
                {
                    Comments = await comments.ToListAsync(),
                    CommentsCount = comments.Count()
                });
            }
        }

        [Route("admin/delete")]
        [Authorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteObjectComment([FromBody] DeleteCommentDto deleteCommentDto)
        {
            var result = await _commentService.AuthorizedDeleteComment(deleteCommentDto);
            return StatusCode(result, new
            {
                Message = "The comment has been deleted"
            });
        }
    }
}
