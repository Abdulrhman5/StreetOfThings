using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.CommentsQueries
{
    public interface ICommentsGetter
    {
        public Task<CommentListDto> GetCommentsForObject(int objectId);
        Task<CommentListDto> GetCommentsForObject(int objectId, PagingArguments pagingArguments);
    }
}
