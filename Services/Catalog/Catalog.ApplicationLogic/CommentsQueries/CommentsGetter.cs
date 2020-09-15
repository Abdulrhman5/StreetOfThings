using Catalog.DataAccessLayer;
using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;

namespace Catalog.ApplicationLogic.CommentsQueries
{
    class CommentsGetter : ICommentsGetter
    {
        private IRepository<Guid, ObjectComment> _commentsRepo;

        public CommentsGetter(IRepository<Guid,ObjectComment> commentsRepo)
        {
            _commentsRepo = commentsRepo;
        }

        public async Task<CommentListDto> GetCommentsForObject(int objectId, PagingArguments pagingArguments)
        {
            var comments = from comment in _commentsRepo.Table
                           where comment.ObjectId == objectId
                           orderby comment.AddedAtUtc descending
                           select new CommentDto
                           {
                               UserId = comment.Login.UserId.ToString(),
                               ObjectId = comment.ObjectId,
                               Comment = comment.Comment,
                               CommentedAtUtc = comment.AddedAtUtc,
                               CommentId = comment.ObjectCommentId
                           };

            return new CommentListDto
            {
                Comments = await comments.SkipTakeAsync(pagingArguments),
                CommentsCount = comments.Count()
            };

        }

        public async Task<CommentListDto> GetCommentsForObject(int objectId)
        {
            var comments = from comment in _commentsRepo.Table
                           where comment.ObjectId == objectId
                           orderby comment.AddedAtUtc descending
                           select new CommentDto
                           {
                               UserId = comment.Login.UserId.ToString(),
                               ObjectId = comment.ObjectId,
                               Comment = comment.Comment,
                               CommentedAtUtc = comment.AddedAtUtc,
                               CommentId = comment.ObjectCommentId
                           };

            return new CommentListDto
            {
                Comments = await comments.ToListAsync(),
                CommentsCount = comments.Count()
            };
        }
    }
}
