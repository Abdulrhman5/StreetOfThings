using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services.ObjectServices
{
    class ObjectDetailsGetter
    { 
        private IRepository<int, OfferedObject> _objectRepo;

        private IPhotoUrlConstructor _photoConstructor;

        private IObjectViewsService _viewsManager;

        private IObjectQueryHelper _queryHelper;

        private IUserDataManager _userDataManager;

        public ObjectDetailsGetter(IRepository<int, OfferedObject> objectRepo,
            IPhotoUrlConstructor photoConstructor,
            IObjectViewsService viewsManager,
            IObjectQueryHelper queryHelper,
            IUserDataManager userDataManager)
        {
            _objectRepo = objectRepo;
            _photoConstructor = photoConstructor;
            _viewsManager = viewsManager;
            _queryHelper = queryHelper;
            _userDataManager = userDataManager;
        }

        public async Task<CommandResult<ObjectDetailsDto>> GetObjectDetails(int objectId)
        {
            var filteredObjects = _objectRepo.Table.Where(_queryHelper.IsValidObject);

            var userId = _userDataManager.GetCuurentUser()?.UserId;

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
                              OwnerId = o.OwnerLogin.User.UserId.ToString(),
                              Photos = o.Photos.Select(op => _photoConstructor.Construct(op)).ToList(),
                              Tags = o.Tags.Select(ot => ot.Tag.Name).ToList(),
                              Type = o.CurrentTransactionType,
                              CommentsCount = o.Comments.Count,
                              LikesCount = o.Likes.Count,
                              IsLikedByMe = o.Likes.Any(like => like.Login.UserId.ToString() == userId),
                              Comments = (from comment in o.Comments
                                          orderby comment.AddedAtUtc
                                          descending
                                          select new CommentDto
                                          {
                                              UserId = comment.Login.UserId.ToString(),
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
