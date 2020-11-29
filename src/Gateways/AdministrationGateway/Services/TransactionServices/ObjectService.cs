using CatalogService.Grpc;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static CatalogService.Grpc.ObjectsGrpc;

namespace AdministrationGateway.Services.TransactionServices
{
    public class ObjectService
    {
        private ObjectsGrpcClient _objectsClient;

        private IConfiguration _configs;
        public ObjectService(IConfiguration configs, ObjectsGrpcClient grpcClient)
        {
            _configs = configs;
            var channel = GrpcChannel.ForAddress(_configs["Services:Grpc:Catalog"]);
            _objectsClient = grpcClient;
        }

        public async Task<List<TransactionObjectDto>> GetObjectsByIds(List<int> objectsIds)
        {

            var objectsIdsModel = new ObjectsIdsModel();
            objectsIds.ForEach(oid => objectsIdsModel.ObjectsIds.Add(oid));

            var upstreamModel = await _objectsClient.GetObjectsDataAsync(objectsIdsModel);

            var objects = upstreamModel.Objects.Select(om => new TransactionObjectDto
            {
                Description = om.Description,
                Id = om.Id,
                Name = om.Name,
                PhotoUrl = om.PhotoUrl
            }).ToList();

            return objects;
        }
    }
}
