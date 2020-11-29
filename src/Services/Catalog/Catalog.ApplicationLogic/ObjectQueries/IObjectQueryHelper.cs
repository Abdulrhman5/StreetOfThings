using Catalog.ApplicationLogic.Infrastructure;
using Catalog.Models;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public interface IObjectQueryHelper
    {
        Expression<Func<OfferedObject, bool>> IsValidObject { get; }
        Expression<Func<OfferedObject, bool>> ValidForFreeAndLendibg { get; }
        Expression<Func<OfferedObject, bool>> DistanceFilter(Point userLocation, int threshold);

        Expression<Func<OfferedObject, ObjectDto>> ObjectDtoSelectExp(IPhotoUrlConstructor photoConstructor);
        Expression<Func<OfferedObject, ObjectDtoV1_1>> ObjectDtoSelectExpV1_1(IPhotoUrlConstructor photoConstructor, string userId, Point userLocation);
        IQueryable<OfferedObject> OrderObject(IQueryable<OfferedObject> objects, Point userLocation, OrderByType orderType);
    }
}