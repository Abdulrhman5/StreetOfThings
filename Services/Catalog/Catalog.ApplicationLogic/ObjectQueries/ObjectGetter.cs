using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary;
using System.Linq.Expressions;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IObjectPhotoUrlConstructor _photoConstructor;

        private IObjectImpressionsManager _impressionManager;

        private ObjectQueryHelper _queryHelper;

        private Expression<Func<OfferedObject, ObjectDto>> ObjectDtoSelectExp { get; set; }
        public ObjectGetter(IRepository<int, OfferedObject> repository,
            IObjectPhotoUrlConstructor photoUrlConstructor,
            IObjectImpressionsManager impressionsManager, ObjectQueryHelper queryHelper)
        {
            _objectRepo = repository;
            _photoConstructor = photoUrlConstructor;
            _impressionManager = impressionsManager;
            _queryHelper = queryHelper;

            ObjectDtoSelectExp = (o) => new ObjectDto
            {
                Id = o.OfferedObjectId,
                CountOfImpressions = o.Impressions.Count,
                CountOfViews = 0,
                Description = o.Description,
                Name = o.Name,
                Rating = null,
                OwnerId = o.OwnerLogin.User.OriginalUserId,
                Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                Type = o.CurrentTransactionType,
            };
        }

        public async Task<List<ObjectDto>> GetObjects(PagingArguments arguments)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          orderby o.OfferedObjectId
                          select new ObjectDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };

            var objectsList = await objects.SkipTakeAsync(arguments);
            await _impressionManager.AddImpressions(objectsList);
            return objectsList;
        }

        public async Task<List<ObjectDto>> GetAllObjects() 
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          orderby o.OfferedObjectId
                          select o;

            var objectsList = await objects.Select(ObjectDtoSelectExp).ToListAsync();
            return objectsList;
        }

        public async Task<ObjectDto> GetObjectById(int objectId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          where o.OfferedObjectId == objectId
                          orderby o.OfferedObjectId
                          select new ObjectDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };

            var objectsList = await objects.FirstOrDefaultAsync();
            return objectsList;
        }


        public async Task<List<ObjectDto>> GetObjectsByIds(List<int> objectsIds)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          where objectsIds.Any(oid => oid == o.OfferedObjectId)
                          orderby o.OfferedObjectId
                          select new ObjectDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };
            return await objects.ToListAsync();
        }  
        
        public async Task<ObjectsForUserListDto> GetObjectsOwnedByUser(string originalUserId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject);

            var objects = from o in filteredObjects
                          where o.OwnerLogin.User.OriginalUserId == originalUserId
                          orderby o.OfferedObjectId
                          select new ObjectDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };
            var freeObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject).Where(_queryHelper.ValidForFreeAndLendibg);
            var availableObjectsCount = freeObjects.Count();
            var reservedObjectsCount = filteredObjects.Count() - availableObjectsCount;

            return new ObjectsForUserListDto()
            {
                Objects = await objects.ToListAsync(),
                AvailableObjectsCount = availableObjectsCount,
                ReservedObjectsCount = reservedObjectsCount
            };
        }
    }
}
