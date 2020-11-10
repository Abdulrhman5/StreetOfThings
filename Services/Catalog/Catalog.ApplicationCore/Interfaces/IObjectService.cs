using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Services;
using Catalog.ApplicationCore.Services.ObjectServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IObjectService
    {
        Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto);
        Task<CommandResult> AuthorizedDelete(DeleteObjectDto objectDto);
        Task<CommandResult> DeleteObject(DeleteObjectDto objectDto);
        Task<List<ObjectDto>> GetObjects(PagingArguments arguments);
        Task<List<ObjectDtoV1_1>> GetObjectsV1_1(PagingArguments arguments);
        Task<ObjectsForAdministrationListDto> GetAllObjects();
        Task<ObjectDto> GetObjectById(int objectId);
        Task<List<ObjectDto>> GetObjectsByIds(List<int> objectsIds);
        Task<ObjectsForUserListDto> GetObjectsOwnedByUser(string originalUserId);
        Task<List<ObjectDtoV1_1>> GetObjects(OrderByType orderBy, PagingArguments pagingArgs);
        Task<CommandResult<ObjectDetailsDto>> GetObjectDetails(int objectId);

    }
}