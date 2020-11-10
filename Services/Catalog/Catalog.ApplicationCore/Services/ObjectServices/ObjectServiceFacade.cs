using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.ObjectServices
{
    class ObjectServiceFacade : IObjectService
    {
        private ObjectGetter _objectGetter;

        private ObjectAdderDeleter _objectAdder8Deleter;

        public ObjectServiceFacade(ObjectGetter objectGetter, ObjectAdderDeleter objectAdder8Deleter)
        {
            _objectGetter = objectGetter;
            _objectAdder8Deleter = objectAdder8Deleter;
        }

        Task<CommandResult<OfferedObject>> IObjectService.AddObject(AddObjectDto objectDto) => _objectAdder8Deleter.AddObject(objectDto);
        Task<CommandResult> IObjectService.AuthorizedDelete(DeleteObjectDto objectDto) => _objectAdder8Deleter.AuthorizedDelete(objectDto);
        Task<CommandResult> IObjectService.DeleteObject(DeleteObjectDto objectDto) => _objectAdder8Deleter.DeleteObject(objectDto);
        Task<ObjectsForAdministrationListDto> IObjectService.GetAllObjects() => _objectGetter.GetAllObjects();
        Task<ObjectDto> IObjectService.GetObjectById(int objectId) => _objectGetter.GetObjectById(objectId);
        Task<List<ObjectDto>> IObjectService.GetObjects(PagingArguments arguments) => _objectGetter.GetObjects(arguments);
        Task<List<ObjectDtoV1_1>> IObjectService.GetObjects(OrderByType orderBy, PagingArguments pagingArgs) => _objectGetter.GetObjects(orderBy, pagingArgs);
        Task<List<ObjectDto>> IObjectService.GetObjectsByIds(List<int> objectsIds) => _objectGetter.GetObjectsByIds(objectsIds);
        Task<ObjectsForUserListDto> IObjectService.GetObjectsOwnedByUser(string originalUserId) => _objectGetter.GetObjectsOwnedByUser(originalUserId);
        Task<List<ObjectDtoV1_1>> IObjectService.GetObjectsV1_1(PagingArguments arguments) => _objectGetter.GetObjectsV1_1(arguments);
    }
}
