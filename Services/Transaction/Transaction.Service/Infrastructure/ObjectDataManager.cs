using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.Service.Models;

namespace Transaction.Service.Infrastructure
{
    public class ObjectDataManager
    {

        private readonly IRepository<int, OfferedObject> _objectsRepo;
        private readonly IRemotlyObjectGetter _objectGetter;

        public ObjectDataManager(IRepository<int, OfferedObject> objectsRepo,
            IRemotlyObjectGetter objectGetter)
        {
            _objectsRepo = objectsRepo;
            _objectGetter = objectGetter;
        }

        public async Task<OfferedObject?> GetObjectAsync(int objectId)
        {
            var existingObject = _objectsRepo.Table.Where(o => o.OriginalObjectId == objectId).FirstOrDefault();
            if(existingObject is object)
            {
                return existingObject;
            }

            var remoteObject = await _objectGetter.GetObject(objectId).ConfigureAwait(false);
            if(remoteObject is null)
            {
                return null;
            }

            var addingResult = _objectsRepo.Add(remoteObject);

            await _objectsRepo.SaveChangesAsync();
            return addingResult;
        }

    }
}
