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

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IObjectPhotoUrlConstructor _photoConstructor;

        private IObjectImpressionsManager _impressionManager;

        private ObjectQueryHelper _queryHelper;
        public ObjectGetter(IRepository<int, OfferedObject> repository,
            IObjectPhotoUrlConstructor photoUrlConstructor,
            IObjectImpressionsManager impressionsManager, ObjectQueryHelper queryHelper)
        {
            _objectRepo = repository;
            _photoConstructor = photoUrlConstructor;
            _impressionManager = impressionsManager;
            _queryHelper = queryHelper;
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
    }
}
