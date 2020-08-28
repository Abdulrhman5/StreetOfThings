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

        public async Task<List<CommentDto>> GetCommentsForObject(int objectId, PagingArguments pagingArguments)
        {
            var comments = from comment in _commentsRepo.Table
                           where comment.ObjectId == objectId
                           select new CommentDto
                           {
                               UserId = comment.Login.User.OriginalUserId,
                               ObjectId = comment.ObjectId,
                               Comment = comment.Comment,
                               CommentedAtUtc = comment.AddedAtUtc,
                               CommentId = comment.ObjectCommentId
                           };

            return await comments.SkipTakeAsync(pagingArguments);

        }

        public async Task<List<CommentDto>> GetCommentsForObject(int objectId)
        {
            var comments = from comment in _commentsRepo.Table
                           where comment.ObjectId == objectId
                           select new CommentDto
                           {
                               UserId = comment.Login.User.OriginalUserId,
                               ObjectId = comment.ObjectId,
                               Comment = comment.Comment,
                               CommentedAtUtc = comment.AddedAtUtc,
                               CommentId = comment.ObjectCommentId
                           };

            return await comments.ToListAsync();
        }
    }
}
