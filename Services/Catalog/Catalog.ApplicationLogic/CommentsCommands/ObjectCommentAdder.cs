using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Catalog.ApplicationLogic.CommentsCommands
{
    class ObjectCommentAdder : IObjectCommentAdder
    {
        private IRepository<Guid, ObjectComment> _commentRepo;

        private UserDataManager _userDataManager;

        private IRepository<int, OfferedObject> _objectRepo;

        private ILogger<ObjectCommentAdder> _logger;
        public ObjectCommentAdder(IRepository<Guid,ObjectComment> commentRepo,
            UserDataManager userDataManager,
            IRepository<int, OfferedObject> objectRepo,
            ILogger<ObjectCommentAdder> logger)
        {
            _commentRepo = commentRepo;
            _userDataManager = userDataManager;
            _objectRepo = objectRepo;
            _logger = logger;
        }

        public async Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment)
        {
            var login = await _userDataManager.AddCurrentUserIfNeeded();
            if(login.Item1 is null)
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }

            if(comment is null)
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
            if(@object is null) 
            {
                return new CommandResult<ObjectComment>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.COMMENT.OBJECT.NOT.EXISTS",
                    Message = "The object you are trying to add comment to dose not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
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
            catch(Exception e)
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
