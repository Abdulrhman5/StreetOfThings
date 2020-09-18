using Catalog.ApplicationLogic.Infrastructure;
using Catalog.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectQueryHelper : IObjectQueryHelper
    {
        public Expression<Func<OfferedObject, bool>> ValidForFreeAndLendibg => (o) =>
            // The object is actually is at the owner
            o.Transactions.All(t => t.ReturnId != null || (t.ReceivingId == null && t.ReturnId == null && t.Status == TransactionStatus.Cancelled));


        public Expression<Func<OfferedObject, bool>> IsValidObject => (o) =>
        o.ObjectStatus == ObjectStatus.Available &&
        // if object has endDate and it is after now Or has not endDate
        (o.EndsAt > DateTime.UtcNow || !o.EndsAt.HasValue) &&
        // The owner is available
        o.OwnerLogin.User.Status == UserStatus.Available;

        public Expression<Func<OfferedObject, bool>> DistanceFilter(Point userLocation, int threshold) =>
            o => o.OwnerLogin.User.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault().LoginLocation.Distance(userLocation) < threshold;
        public Expression<Func<OfferedObject, ObjectDto>> ObjectDtoSelectExp(IPhotoUrlConstructor photoConstructor) => (o) => new ObjectDto
        {
            Id = o.OfferedObjectId,
            CountOfImpressions = o.Impressions.Count,
            CountOfViews = 0,
            Description = o.Description,
            Name = o.Name,
            Rating = null,
            OwnerId = o.OwnerLogin.UserId.ToString(),
            Photos = o.Photos.Select(op => photoConstructor.Construct(op)).ToList(),
            Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
            Type = o.CurrentTransactionType,
        };

        public Expression<Func<OfferedObject, ObjectDtoV1_1>> ObjectDtoSelectExpV1_1(IPhotoUrlConstructor photoConstructor, string userId, Point userLocation) => (o) => new ObjectDtoV1_1
        {
            Id = o.OfferedObjectId,
            CountOfImpressions = o.Impressions.Count,
            CountOfViews = 0,
            Description = o.Description,
            Name = o.Name,
            Rating = null,
            OwnerId = o.OwnerLogin.UserId.ToString(),
            Photos = o.Photos.Select(op => photoConstructor.Construct(op)).ToList(),
            Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
            Type = o.CurrentTransactionType,
            CommentsCount = o.Comments.Count,
            IsLikedByMe = o.Likes.Any(like => like.Login.UserId.ToString() == userId),
            LikesCount = o.Likes.Count,
            DistanceInMeters = o.OwnerLogin.User.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault().LoginLocation.Distance(userLocation)
        };

        public IQueryable<OfferedObject> OrderObject(IQueryable<OfferedObject> objects, Point userLocation, OrderByType orderType)
        {
            if (orderType == OrderByType.Date)
            {
                return objects.OrderByDescending(o => o.PublishedAt);
            }
            else if (orderType == OrderByType.MostBorrowed)
            {
                return objects.OrderByDescending(o => o.Transactions.Count / (DateTime.UtcNow - o.PublishedAt).TotalSeconds + 1);
            }
            else if (orderType == OrderByType.MostLiked)
            {
                return objects.OrderByDescending(o => o.Likes.Count / (DateTime.UtcNow - o.PublishedAt).TotalSeconds + 1);
            }
            else if (orderType == OrderByType.Trending)
            {
                return objects.OrderByDescending(o => o.Views.Count / (DateTime.UtcNow - o.PublishedAt).TotalSeconds + 1);
            }
            else if (orderType == OrderByType.Nearest)
            {
                return from o in objects
                       orderby o.OwnerLogin.User.Logins.OrderByDescending(l => l.LoggedAt).FirstOrDefault().LoginLocation.Distance(userLocation)
                       select o;
            }
            else if (orderType == OrderByType.TopRated)
            {
                return objects.OrderByDescending(o => o.Transactions.Where(t => t.Rating.HasValue).Average(t => t.Rating));
            }
            else
            {
                return objects.OrderBy(o => o.OfferedObjectId);
            }
        }
    }
}
