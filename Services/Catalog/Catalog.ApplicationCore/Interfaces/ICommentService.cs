using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface ICommentService
    {
        public Task<CommandResult<ObjectComment>> AddComment(AddCommentDto comment);

        public Task<CommandResult> AuthorizedDeleteComment(DeleteCommentDto deleteCommentDto);

    }
}
