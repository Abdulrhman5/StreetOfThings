using Catalog.ApplicationCore.Interfaces;
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
        private IObjectService _objectService;

        public ObjectService(IObjectService objectService)
        {
            _objectService = objectService;

        }


        public override async Task<ObjectsModel> GetObjectsData(ObjectsIdsModel request, ServerCallContext context)
        {
            var objects = await _objectService.GetObjectsByIds(request.ObjectsIds.ToList());

            var objectsModel = new ObjectsModel();
            objects.ForEach(o => objectsModel.Objects.Add(new ObjectModel
            {
                Id = o.Id,
                Description = o.Description,
                Name = o.Name,
                PhotoUrl = o.Photos.FirstOrDefault()
            }));

            return objectsModel;
        }
    }
}
