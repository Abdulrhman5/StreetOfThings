using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services
{
    public class DeleteCommentDto
    {
        public string CommentId { get; set; }
    }
    class ObjectCommentDeleter
    {
        private IRepository<Guid, ObjectComment> _commentRepo;
        private ILogger<ObjectCommentDeleter> _logger;

        public ObjectCommentDeleter(IRepository<Guid, ObjectComment> commentRepo,
            ILogger<ObjectCommentDeleter> logger)
        {
            _commentRepo = commentRepo;
            _logger = logger;
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
    }
}
