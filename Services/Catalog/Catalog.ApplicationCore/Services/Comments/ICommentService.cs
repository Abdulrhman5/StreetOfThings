using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.Comments
{
    public interface ICommentService
    {
        Task<CommentListDto> GetCommentsForObject(int objectId);
        Task<CommentListDto> GetCommentsForObject(int objectId, PagingArguments pagingArguments);
        public Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment);

        public Task<CommandResult> AuthorizedDeleteComment(DeleteCommentDto deleteCommentDto);

    }
}
