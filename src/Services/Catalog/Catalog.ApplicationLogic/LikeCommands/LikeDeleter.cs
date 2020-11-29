using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.LikeCommands
{
    public interface ILikeDeleter
    {
        Task<CommandResult> Unlike(AddLikeDto removeLikeDto);
    }

    class LikeDeleter : ILikeDeleter
    {
        private IRepository<Guid, ObjectLike> _likesRepo;

        private IRepository<int, OfferedObject> _objectsRepo;

        private UserDataManager _userDataManager;

        public LikeDeleter(IRepository<Guid, ObjectLike> likesRepo, IRepository<int, OfferedObject> objectsRepo, UserDataManager userDataManager)
        {
            _likesRepo = likesRepo;
            _objectsRepo = objectsRepo;
            _userDataManager = userDataManager;
        }

        public async Task<CommandResult> Unlike(AddLikeDto removeLikeDto)
        {
            if (removeLikeDto is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.NULL",
                    Message = "Please send valid information",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            }
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            if (user?.UserId is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.REMOVE.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                });
            }

            var @object = _objectsRepo.Get(removeLikeDto.ObjectId);
            if (@object is null || @object.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.REMOVE.UNAVAILABLE",
                    Message = "This object is unavailable",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var previousLikes = _likesRepo.Table.Where(ol => ol.Login.UserId == user.UserId && ol.ObjectId == removeLikeDto.ObjectId).ToList();
            if (previousLikes.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.REMOVE.NO.PREVIOUS.LIKE",
                    Message = "You have not liked this object before",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            _likesRepo.Delete(previousLikes.SingleOrDefault());
            await _likesRepo.SaveChangesAsync();
            return new CommandResult();
        }
    }
}
