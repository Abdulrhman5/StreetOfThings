﻿using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IObjectPhotoUrlConstructor _photoConstructor;

        private IObjectImpressionsManager _impressionManager;
        public ObjectGetter(IRepository<int, OfferedObject> repository, IObjectPhotoUrlConstructor photoUrlConstructor, IObjectImpressionsManager impressionsManager)
        {
            _objectRepo = repository;
            _photoConstructor = photoUrlConstructor;
            _impressionManager = impressionsManager;
        }

        public async Task<List<ObjectDto>> GetObjects(PagingArguments arguments)
        {
            var objects = from o in _objectRepo.Table
                          // If EndsAt has value and it is valid or EndsAt has not value
                          where ((o.EndsAt.HasValue && o.EndsAt > DateTime.UtcNow ) || (!o.EndsAt.HasValue)) &&
                          
                          o.CurrentTransactionType == TransactionType.Free ?
                          // The object is not taken
                          !o.ObjectFreeProperties.TakenAtUtc.HasValue :
                          // The object is not currently loaned
                          o.ObjectLoanProperties.ObjectLoans.All(ol => ol.LoanEndAt < DateTime.UtcNow)


                          orderby o.OfferedObjectId
                          select new ObjectDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.Owner.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select( ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };

            var objectsList = await objects.SkipTakeAsync(arguments);
            await _impressionManager.AddImpressions(objectsList);
            return objectsList;
                          
        }
    }
}
