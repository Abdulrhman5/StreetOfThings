using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    class ObjectsOrderedGetter : IObjectsOrderedGetter
    {
        private IObjectQueryHelper _queryHelper;

        private CurrentUserCredentialsGetter _credentialsGetter;

        private IRepository<int, OfferedObject> _objectRepo;

        private IPhotoUrlConstructor _photoUrlConstructor;

        private int IncludeObjectLessThan;

        public ObjectsOrderedGetter(IObjectQueryHelper queryHelper,
            CurrentUserCredentialsGetter credentialsGetter,
            IRepository<int, OfferedObject> objectRepo,
            IPhotoUrlConstructor photoUrlConstructor,
            IConfiguration configs)
        {
            _queryHelper = queryHelper;
            _credentialsGetter = credentialsGetter;
            _objectRepo = objectRepo;
            _photoUrlConstructor = photoUrlConstructor;
            IncludeObjectLessThan = int.Parse(configs["Settings:IncludeObjectLessThan"]);
        }

        public async Task<List<ObjectDtoV1_1>> GetObjects(OrderByType orderBy, PagingArguments pagingArgs)
        {
            var userLocation = null as Point;

            var userId = _credentialsGetter.GetCuurentUser()?.UserId;

            var selectExp = _queryHelper.ObjectDtoSelectExpV1_1(_photoUrlConstructor, userId, userLocation);

            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg)
                .Where(_queryHelper.DistanceFilter(userLocation, IncludeObjectLessThan));

            var orderResult = _queryHelper.OrderObject(filteredObjects, userLocation, orderBy);
            var objectList = await orderResult
                .Select(selectExp)
                .SkipTakeAsync(pagingArgs);
            return objectList;
        }
    }

    public enum OrderByType
    {
        Default,
        Nearest,
        MostLiked,
        Date,
        TopRated,
        MostBorrowed,
        Trending
    }
}
