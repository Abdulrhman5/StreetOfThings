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

        private IUserDataManager _userDataManager;

        public ObjectsOrderedGetter(IObjectQueryHelper queryHelper,
            CurrentUserCredentialsGetter credentialsGetter,
            IRepository<int, OfferedObject> objectRepo,
            IPhotoUrlConstructor photoUrlConstructor,
            IConfiguration configs, IUserDataManager userDataManager)
        {
            _queryHelper = queryHelper;
            _credentialsGetter = credentialsGetter;
            _objectRepo = objectRepo;
            _photoUrlConstructor = photoUrlConstructor;
            IncludeObjectLessThan = int.Parse(configs["Settings:IncludeObjectLessThan"]);
            _userDataManager = userDataManager;
        }

        public async Task<List<ObjectDtoV1_1>> GetObjects(OrderByType orderBy, PagingArguments pagingArgs)
        {
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            var userId = user.UserId;
            var userLocation = login.LoginLocation;

            var selectExp = _queryHelper.ObjectDtoSelectExpV1_1(_photoUrlConstructor, userId.ToString(), userLocation);

            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg)
                .Where(_queryHelper.DistanceFilter(userLocation, IncludeObjectLessThan));
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            var x = (from o in _objectRepo.Table 
                     select new
                     {
                         dd = o.Views.Count / (double) (DbF.DateDiffSecond(o.PublishedAt,DateTime.UtcNow) + 1),
                         c = o.Views.Count,
                         diff = (DbF.DateDiffSecond(o.PublishedAt, DateTime.UtcNow) + 1),
                         id = o.OfferedObjectId
                     })
                .ToList();
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
