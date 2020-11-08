using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.Comments
{
    class CommentService
    {

        private ILogger<ObjectCommentDeleter> _logger;

        private IRepository<Guid, ObjectComment> _commentRepo;

        private IUserDataManager _userDataManager;

        private IRepository<int, OfferedObject> _objectRepo;

        public CommentService(ILogger<ObjectCommentDeleter> logger, 
            IRepository<Guid, ObjectComment> commentRepo, 
            IUserDataManager userDataManager, 
            IRepository<int, OfferedObject> objectRepo)
        {
            _logger = logger;
            _commentRepo = commentRepo;
            _userDataManager = userDataManager;
            _objectRepo = objectRepo;
        }

        public async Task<CommentListDto> GetCommentsForObject(int objectId, PagingArguments pagingArguments)
        {
            var comments = from comment in _commentRepo.Table
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
            var comments = from comment in _commentRepo.Table
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

        public async Task<CommandResult> AuthorizedDeleteComment(DeleteCommentDto deleteCommentDto)
        {
            if (deleteCommentDto == null || deleteCommentDto.CommentId.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.COMMENT.DELETE.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (!Guid.TryParse(deleteCommentDto.CommentId, out var commentIdGuid))
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.COMMENT.DELETE.INVALID.ID",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var comment = _commentRepo.Table.SingleOrDefault(c => c.ObjectCommentId == commentIdGuid);
            if (comment == null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.COMMENT.DELETE.INVALID.ID",
                    Message = "The comment you are trying to delete does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            try
            {
                _commentRepo.Delete(comment);
                await _commentRepo.SaveChangesAsync();
                return new CommandResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"There were a problem deleting the object:{deleteCommentDto.CommentId}");
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.DELETE.INTERNAL.ERROR",
                    Message = "There were an error deleting your object",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }
            
        public async Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment)
        {
            var login = await _userDataManager.AddCurrentUserIfNeeded();
            if (login.Item1 is null)
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            if (comment is null)
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.NULL",
                    Message = "Please fill out the fields",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (comment.Comment.IsNullOrEmpty())
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.COMMENT.NULL",
                    Message = "Please fill the comment field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }
            var @object = _objectRepo.Get(comment.ObjectId);
            if (@object is null)
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.OBJECT.NOT.EXISTS",
                    Message = "The object you are trying to add comment to dose not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (@object is null || @object.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.DOES.NOT.EXISTS",
                    Message = "You are not authorized to add a photo to this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            var commentModel = new ObjectComment
            {
                ObjectCommentId = Guid.NewGuid(),
                AddedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
                Comment = comment.Comment,
                ObjectId = comment.ObjectId,
                LoginId = login.Item1.LoginId
            };

            try
            {
                var model = _commentRepo.Add(commentModel);
                await _commentRepo.SaveChangesAsync();
                return new CommandResult<ObjectComment>(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"There were an error while adding comment to object:{comment.ObjectId}, comment:{comment.Comment}");
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.INTERNAL.SERVER.ERROR",
                    Message = "There were an error while trying to add a comment",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }
        }
    }
}
