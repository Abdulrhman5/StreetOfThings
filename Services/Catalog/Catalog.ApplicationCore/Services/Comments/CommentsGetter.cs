using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Services.Comments;
using Catalog.ApplicationCore.Dtos;

namespace Catalog.ApplicationCore.Services
{
    class CommentsGetter
    {
        private IRepository<Guid, ObjectComment> _commentsRepo;

        public CommentsGetter(IRepository<Guid, ObjectComment> commentsRepo)
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
                Comments = comments.SkipTake(pagingArguments),
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
                Comments = comments.ToList(),
                CommentsCount = comments.Count()
            };
        }
    }
}
