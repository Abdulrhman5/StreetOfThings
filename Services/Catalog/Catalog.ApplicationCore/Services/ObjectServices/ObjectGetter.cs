using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.ObjectServices
{
    class ObjectGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IPhotoUrlConstructor _photoConstructor;

        private IObjectImpressionsManager _impressionManager;

        private IObjectQueryHelper _queryHelper;
        private Expression<Func<OfferedObject, ObjectDto>> ObjectDtoSelectExp { get; set; }

        private IConfiguration _configs;

        private IUserDataManager _userDataManager;
        private int IncludeObjectLessThan => int.Parse(_configs["Settings:IncludeObjectLessThan"]);
        public ObjectGetter(IRepository<int, OfferedObject> repository,
            IPhotoUrlConstructor photoUrlConstructor,
            IObjectImpressionsManager impressionsManager, IObjectQueryHelper queryHelper,
            IConfiguration configs, IUserDataManager userDataManager)
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
                OwnerId = o.OwnerLogin.UserId.ToString(),
                Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                Type = o.CurrentTransactionType,
            };
            _configs = configs;
            _userDataManager = userDataManager;
        }

        public async Task<List<ObjectDto>> GetObjects(PagingArguments arguments)
        {
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            var userLocation = login.LoginLocation;
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          let distance = o.OwnerLogin.User.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault().LoginLocation.Distance(userLocation)
                          orderby o.OfferedObjectId
                          select o;

            var objectsList = await objects.Select(ObjectDtoSelectExp).SkipTakeAsync(arguments);
            await _impressionManager.AddImpressions(objectsList);
            return objectsList;
        }

        public async Task<List<ObjectDtoV1_1>> GetObjectsV1_1(PagingArguments arguments)
        {
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            var userId = user.UserId;
            var userLocation = login.LoginLocation;

            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);


            var objects = from o in filteredObjects
                          let distance = o.OwnerLogin.User.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault().LoginLocation.Distance(userLocation)
                          where distance <= IncludeObjectLessThan
                          orderby o.OfferedObjectId
                          select new ObjectDtoV1_1
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.UserId.ToString(),
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                              CommentsCount = o.Comments.Count,
                              LikesCount = o.Likes.Count,
                              IsLikedByMe = o.Likes.Any(like => like.Login.UserId == userId),
                              DistanceInMeters = distance
                          };

            var objectsList = await objects.SkipTakeAsync(arguments);
            await _impressionManager.AddImpressions(objectsList.Select(o => o.Id).ToList());
            return objectsList;
        }

        public async Task<ObjectsForAdministrationListDto> GetAllObjects()
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject);

            var objects = from o in filteredObjects
                          orderby o.OfferedObjectId
                          select o;

            var objectsList = await objects.Select(ObjectDtoSelectExp).ToListAsync();
            return new ObjectsForAdministrationListDto
            {
                LendingObjectsCount = objects.Count(o => o.CurrentTransactionType == TransactionType.Lending),
                FreeObjectsCount = objects.Count(o => o.CurrentTransactionType == TransactionType.Free),
                RentingObjectsCount = 0,
                Objects = objectsList
            };
        }

        public async Task<ObjectDto> GetObjectById(int objectId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var objects = from o in filteredObjects
                          where o.OfferedObjectId == objectId
                          orderby o.OfferedObjectId
                          select o;

            var objectsList = await objects.Select(ObjectDtoSelectExp).FirstOrDefaultAsync();
            return objectsList;
        }


        public async Task<List<ObjectDto>> GetObjectsByIds(List<int> objectsIds)
        {
            var filteredObjects = _objectRepo.Table;

            var objects = from o in filteredObjects
                          where objectsIds.Any(oid => oid == o.OfferedObjectId)
                          orderby o.OfferedObjectId
                          select o;
            return await objects.Select(ObjectDtoSelectExp).ToListAsync();
        }

        public async Task<ObjectsForUserListDto> GetObjectsOwnedByUser(string originalUserId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject);

            var objects = from o in filteredObjects
                          where o.OwnerLogin.UserId.ToString() == originalUserId
                          orderby o.OfferedObjectId
                          select o;

            var freeObjects = _objectRepo.Table
                .Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg)
                .Where(o => o.OwnerLogin.UserId.ToString() == originalUserId);

            var availableObjectsCount = freeObjects.Count();
            var reservedObjectsCount = objects.Count() - availableObjectsCount;

            return new ObjectsForUserListDto()
            {
                Objects = await objects.Select(ObjectDtoSelectExp).ToListAsync(),
                AvailableObjectsCount = availableObjectsCount,
                ReservedObjectsCount = reservedObjectsCount
            };
        }
    }
}
