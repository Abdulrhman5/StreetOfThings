using CommonLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public interface IObjectGetter
    {
        Task<ObjectsForAdministrationListDto> GetAllObjects();
        Task<ObjectDto> GetObjectById(int objectId);
        Task<List<ObjectDto>> GetObjects(PagingArguments arguments);
        Task<List<ObjectDto>> GetObjectsByIds(List<int> objectsIds);
        Task<ObjectsForUserListDto> GetObjectsOwnedByUser(string originalUserId);
        Task<List<ObjectDtoV1_1>> GetObjectsV1_1(PagingArguments arguments);
    }
}