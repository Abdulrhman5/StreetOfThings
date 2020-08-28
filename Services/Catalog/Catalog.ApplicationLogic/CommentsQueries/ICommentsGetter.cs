using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.CommentsQueries
{
    public interface ICommentsGetter
    {
        public Task<List<CommentDto>> GetCommentsForObject(int objectId);
        Task<List<CommentDto>> GetCommentsForObject(int objectId, PagingArguments pagingArguments);
    }
}
