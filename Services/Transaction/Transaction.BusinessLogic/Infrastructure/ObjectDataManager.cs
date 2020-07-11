using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.Infrastructure
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
            var existingObject = _objectsRepo.Get(objectId);
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
