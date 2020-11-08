using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.Comments
{
    class CommentServiceFacade : ICommentService
    {
        private readonly CommentsGetter _commentsGetter;

        private readonly ObjectCommentAdder _commentAdder;

        private readonly ObjectCommentDeleter _commentDeleter;

        public CommentServiceFacade(CommentsGetter commentsGetter, ObjectCommentAdder commentAdder, ObjectCommentDeleter commentDeleter)
        {
            _commentsGetter = commentsGetter;
            _commentAdder = commentAdder;
            _commentDeleter = commentDeleter;
        }

        public async Task<CommentListDto> GetCommentsForObject(int objectId) => await _commentsGetter.GetCommentsForObject(objectId);
        public async Task<CommentListDto> GetCommentsForObject(int objectId, PagingArguments pagingArguments) => await _commentsGetter.GetCommentsForObject(objectId, pagingArguments);
        public async Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment) => await _commentAdder.AddComment(comment);
        public async Task<CommandResult> AuthorizedDeleteComment(DeleteCommentDto deleteCommentDto) => await _commentDeleter.AuthorizedDeleteComment(deleteCommentDto);

    }
}
