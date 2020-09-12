using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    class ObjectGetter : IObjectGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IPhotoUrlConstructor _photoConstructor;

        private IObjectImpressionsManager _impressionManager;

        private ObjectQueryHelper _queryHelper;

        private CurrentUserCredentialsGetter _credentialsGetter;
        private Expression<Func<OfferedObject, ObjectDto>> ObjectDtoSelectExp { get; set; }

        private IConfiguration _configs;

        private IUserDataManager _userDataManager;
        private int IncludeObjectLessThan => int.Parse(_configs["Settings:IncludeObjectLessThan"]);
        public ObjectGetter(IRepository<int, OfferedObject> repository,
            IPhotoUrlConstructor photoUrlConstructor,
            IObjectImpressionsManager impressionsManager, ObjectQueryHelper queryHelper,
            CurrentUserCredentialsGetter credentialsGetter, IConfiguration configs, IUserDataManager userDataManager)
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
            _credentialsGetter = credentialsGetter;
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
                          where  distance <= IncludeObjectLessThan
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

        public async Task<List<ObjectDtoV1_1>> GetObjectsV1_1(PagingArguments arguments)
        {
            var userLocation = null as Point;
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var userId = _credentialsGetter.GetCuurentUser()?.UserId;

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
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                              CommentsCount = o.Comments.Count,
                              LikesCount = o.Likes.Count,
                              IsLikedByMe = o.Likes.Any(like => like.Login.User.OriginalUserId == userId)
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
            var freeObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject).Where(_queryHelper.ValidForFreeAndLendibg).Where(o => o.OwnerLogin.User.OriginalUserId == originalUserId);
            var availableObjectsCount = freeObjects.Count();
            var reservedObjectsCount = objects.Count() - availableObjectsCount;

            return new ObjectsForUserListDto()
            {
                Objects = await objects.ToListAsync(),
                AvailableObjectsCount = availableObjectsCount,
                ReservedObjectsCount = reservedObjectsCount
            };
        }
    }
}
