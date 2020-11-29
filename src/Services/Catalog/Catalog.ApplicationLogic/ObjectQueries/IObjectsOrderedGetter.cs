using CommonLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public interface IObjectsOrderedGetter
    {
        Task<List<ObjectDtoV1_1>> GetObjects(OrderByType orderBy, PagingArguments pagingArgs);
    }
}