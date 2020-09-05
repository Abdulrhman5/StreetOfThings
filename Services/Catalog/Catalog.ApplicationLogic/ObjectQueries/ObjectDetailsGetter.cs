using Catalog.ApplicationLogic.CommentsQueries;
using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    class ObjectDetailsGetter : IObjectDetailsGetter
    {
        private IRepository<int, OfferedObject> _objectRepo;

        private IObjectPhotoUrlConstructor _photoConstructor;

        private IObjectViewsManager _viewsManager;

        private ObjectQueryHelper _queryHelper;

        private CurrentUserCredentialsGetter _credentialsGetter;

        public ObjectDetailsGetter(IRepository<int, OfferedObject> objectRepo,
            IObjectPhotoUrlConstructor photoConstructor,
            IObjectViewsManager viewsManager,
            ObjectQueryHelper queryHelper,
            CurrentUserCredentialsGetter credentialsGetter)
        {
            _objectRepo = objectRepo;
            _photoConstructor = photoConstructor;
            _viewsManager = viewsManager;
            _queryHelper = queryHelper;
            _credentialsGetter = credentialsGetter;
        }

        public async Task<CommandResult<ObjectDetailsDto>> GetObjectDetails(int objectId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject)
                .Where(_queryHelper.ValidForFreeAndLendibg);

            var userId = _credentialsGetter.GetCuurentUser()?.UserId;


            var objects = from o in filteredObjects
                          where objectId == o.OfferedObjectId
                          orderby o.OfferedObjectId
                          select new ObjectDetailsDto
                          {
                              Id = o.OfferedObjectId,
                              CountOfImpressions = o.Impressions.Count,
                              CountOfViews = o.Views.Count,
                              Description = o.Description,
                              Name = o.Name,
                              Rating = null,
                              OwnerId = o.OwnerLogin.User.OriginalUserId,
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                              CommentsCount = o.Comments.Count,
                              LikesCount = o.Likes.Count,
                              IsLikedByMe = o.Likes.Any(like => like.Login.User.OriginalUserId == userId),
                              Comments = (from comment in o.Comments
                                          orderby comment.AddedAtUtc
                                          descending
                                          select new CommentDto
                                          {
                                              UserId = comment.Login.User.OriginalUserId,
                                              ObjectId = comment.ObjectId,
                                              Comment = comment.Comment,
                                              CommentedAtUtc = comment.AddedAtUtc,
                                              CommentId = comment.ObjectCommentId
                                          }).Take(10).ToList(),
                          };
            var @object = await objects.SingleOrDefaultAsync();

            if (@object is null)
            {
                return new ErrorMessage
                {
                    Message = "The object you requested does not exists.",
                    ErrorCode = "CATALOG.OBJECT.DETAILS.NOTFOUND",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<ObjectDetailsDto>();


            }
            _ = _viewsManager.AddView(@object.Id);
            return new CommandResult<ObjectDetailsDto>(@object);
        }

    }
}
