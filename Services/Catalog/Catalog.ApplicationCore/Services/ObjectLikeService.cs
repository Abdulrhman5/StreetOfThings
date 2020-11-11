using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Services
{
    class ObjectLikeService : IObjectLikeService
    {
        private IUserDataManager _userDataManager;

        private IRepository<Guid, ObjectLike> _likesRepo;

        private IRepository<int, OfferedObject> _objectsRepo;

        public ObjectLikeService(IUserDataManager userDataManager, 
            IRepository<Guid, ObjectLike> likesRepo,
            IRepository<int, OfferedObject> objectsRepo)
        {
            _userDataManager = userDataManager;
            _likesRepo = likesRepo;
            _objectsRepo = objectsRepo;
        }

        public async Task<CommandResult> AddLike(AddLikeDto addLikeDto)
        {
            if (addLikeDto is null)
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
                    ErrorCode = "CATALOG.LIKE.ADD.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                });
            }

            var @object = _objectsRepo.Get(addLikeDto.ObjectId);
            if (@object is null || @object.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.UNAVAILABLE",
                    Message = "This object is unavailable",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            var previousLikes = _likesRepo.Table.Where(ol => ol.Login.UserId == user.UserId && ol.ObjectId == addLikeDto.ObjectId).ToList();
            if (!previousLikes.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.ALREADY.LIKED",
                    Message = "You already liked this object",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });
            }

            _likesRepo.Add(new ObjectLike
            {
                LikedAtUtc = DateTime.UtcNow,
                ObjectLikeId = Guid.NewGuid(),
                LoginId = login.LoginId,
                ObjectId = addLikeDto.ObjectId
            });

            await _likesRepo.SaveChangesAsync();
            return new CommandResult();

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
