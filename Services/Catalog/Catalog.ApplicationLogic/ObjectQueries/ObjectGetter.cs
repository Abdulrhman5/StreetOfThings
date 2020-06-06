using Catalog.ApplicationLogic.Infrastructure;
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

        private ObjectPhotoUrlConstructor _photoConstructor;
        public ObjectGetter(IRepository<int, OfferedObject> repository)
        {
            _objectRepo = repository;
        }

        public List<ObjectDto> GetObjects()
        {
            var objects = from o in _objectRepo.Table
                          where o.CurrentTransactionType == TransactionType.Free ?
                          // The object is not taken
                          o.ObjectFreeProperties.TakenAtUtc.HasValue :
                          // The object is not currently loaned
                          o.ObjectLoanProperties.ObjectLoans.All(ol => ol.LoanEndAt < DateTime.UtcNow)
                          select new ObjectDto
                          {
                              CountOfImpressions = 0,
                              CountOfViews = 0,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.Owner.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select( ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                          };

            return objects.ToList();
                          
        }
    }
}
