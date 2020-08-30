using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.AspNetCore.Mvc.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.LikeCommands
{
    public class LikeAdder
    {
        private IRepository<Guid, ObjectLike> _likesRepository;

        private UserDataManager _userDataManager;

        private IRepository<int, OfferedObject> _objectsRepo;


        public async Task<CommandResult> AddLike(AddLikeDto addLikeDto)
        {
            if(addLikeDto is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.NULL",
                    Message = "Please send valid information",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                });

            }
            var (login, user) = await _userDataManager.AddCurrentUserIfNeeded();
            if(user?.UserId is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.UNAUTHORIZED",
                    Message = "You are not authorized to execute this request",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                });
            }

            var @object = _objectsRepo.Get(addLikeDto.ObjectId);
            if(@object is null || @object.ObjectStatus != ObjectStatus.Available)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.UNAVAILABLE",
                    Message = "This object is unavailable",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                });
            }

            var previousLikes = _likesRepository.Table.Where(ol => ol.Login.User.OriginalUserId == user.OriginalUserId && ol.ObjectId == addLikeDto.ObjectId).ToList();
            if (!previousLikes.IsNullOrEmpty())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.LIKE.ADD.ALREADY.LIKED",
                    Message = "You already liked this object",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                });
            }

            _likesRepository.Add(new ObjectLike
            {
                LikedAtUtc = DateTime.UtcNow,
                ObjectLikeId = Guid.NewGuid(),
                LoginId = login.LoginId,
                ObjectId = addLikeDto.ObjectId
            });

            await _likesRepository.SaveChangesAsync();
            return new CommandResult();

        }
    }
}
