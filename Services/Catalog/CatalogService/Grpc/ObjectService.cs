using Catalog.ApplicationLogic.ObjectQueries;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CatalogService.Grpc.ObjectsGrpc;

namespace CatalogService.Grpc
{
    public class ObjectService : ObjectsGrpcBase
    {
        private IObjectGetter _objectGetter;

        public ObjectService(IObjectGetter objectGetter)
        {
            _objectGetter = objectGetter;

        }


        public override async Task<ObjectsModel> GetObjectsData(ObjectsIdsModel request, ServerCallContext context)
        {
            var objects = await _objectGetter.GetObjectsByIds(request.ObjectsIds.ToList());

            var objectsModel = new ObjectsModel();
            objects.ForEach(o => objectsModel.Objects.Add(new ObjectModel
            {
                Id = o.Id,
                Description = o.Description,
                Name = o.Name
            }));

            return objectsModel;
        }
    }
}
