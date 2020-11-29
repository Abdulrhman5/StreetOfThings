using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Catalog.ApplicationCore.Services.ObjectServices;
using Catalog.Infrastructure.Events;
using EventBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    class EventedObjectService : IObjectService
    {
        private IObjectService _objectService;

        private IEventBus _eventBus;

        public EventedObjectService(IObjectService objectService, IEventBus eventBus)
        {
            _objectService = objectService;
            _eventBus = eventBus;
        }

        public Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto)
        {
            return _objectService.AddObject(objectDto);
        }

        public async Task<CommandResult> AuthorizedDelete(DeleteObjectDto objectDto)
        {
            var result = await _objectService.AuthorizedDelete(objectDto);
            if (result.IsSuccessful)
            {
                PublishEvent(objectDto.ObjectId);
            }
            return result;
        }

        public async Task<CommandResult> DeleteObject(DeleteObjectDto objectDto)
        {
            var result = await _objectService.DeleteObject(objectDto);
            if (result.IsSuccessful)
            {
                PublishEvent(objectDto.ObjectId);
            }
            return result;

        }

        public Task<ObjectsForAdministrationListDto> GetAllObjects()
        {
            return _objectService.GetAllObjects();
        }

        public Task<ObjectDto> GetObjectById(int objectId)
        {
            return _objectService.GetObjectById(objectId);
        }

        public Task<CommandResult<ObjectDetailsDto>> GetObjectDetails(int objectId)
        {
            return _objectService.GetObjectDetails(objectId);
        }

        public Task<List<ObjectDto>> GetObjects(PagingArguments arguments)
        {
            return _objectService.GetObjects(arguments);
        }

        public Task<List<ObjectDtoV1_1>> GetObjects(OrderByType orderBy, PagingArguments pagingArgs)
        {
            return _objectService.GetObjects(orderBy, pagingArgs);
        }

        public Task<List<ObjectDto>> GetObjectsByIds(List<int> objectsIds)
        {
            return _objectService.GetObjectsByIds(objectsIds);
        }

        public Task<ObjectsForUserListDto> GetObjectsOwnedByUser(string originalUserId)
        {
            return _objectService.GetObjectsOwnedByUser(originalUserId);
        }

        public Task<List<ObjectDtoV1_1>> GetObjectsV1_1(PagingArguments arguments)
        {
            return _objectService.GetObjectsV1_1(arguments);
        }

        private void PublishEvent(int objectId)
        {
            var integrationEvent = new ObjectDeletedIntegrationEvent
            {
                Id = Guid.NewGuid(),
                ObjectId = objectId,
                DeletedAtUtc = DateTime.UtcNow,
                OccuredAt = DateTime.UtcNow
            };
            _eventBus.Publish(integrationEvent);
        }
    }
}
