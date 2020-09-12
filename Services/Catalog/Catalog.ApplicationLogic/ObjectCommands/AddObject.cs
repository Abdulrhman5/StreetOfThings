using Catalog.ApplicationLogic.Infrastructure;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    class ObjectAdder : IObjectAdder
    {
        private IRepository<int, Tag> _tagRepo;
        private UserDataManager _userDataManager;
        private IRepository<int, OfferedObject> _objectRepo;

        public ObjectAdder(IRepository<int, Tag> tagRepo,
            IRepository<int,OfferedObject> objectRepo,
            UserDataManager userDataManager)
        {
            _tagRepo = tagRepo;
            _userDataManager = userDataManager;
            _objectRepo = objectRepo;
        }

        public async Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto)
        {
            if (objectDto is null)
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.NULL",
                    Message = "Please fill the field with data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            if (objectDto.ObjectName.IsNullOrEmpty())
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.OBJECTNAME.EMPTY",
                    Message = "Please fill the 'object name' field",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            // Description could be null

            if(objectDto.Tags is null || objectDto.Tags.Count <2)
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.TAGS.TOO.FEW",
                    Message = "Please add at least two tags",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var invalidTags = from t in objectDto.Tags
                              where t.IsNullOrEmpty() || t.Length < 4
                              select t;

            if (invalidTags.Any())
            {
                return new CommandResult<OfferedObject>(new ErrorMessage
                {
                    ErrorCode = "OBJECT.ADD.TAGS.INVALID",
                    Message = "Please send a valid tags",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            var alreadyExistedTags = (from t in _tagRepo.Table
                                     where objectDto.Tags.Any(tt => tt== t.Name)
                                     select t).ToList();

            // No Logic for not existed tags, just discard them.

            var objectTags = alreadyExistedTags.Select(t => new ObjectTag
            {
                TagId = t.TagId
            }).ToList();

            var (login,ownerUser) = await _userDataManager.AddCurrentUserIfNeeded();
            var @object = new OfferedObject
            {
                Description = objectDto.Description,
                Name = objectDto.ObjectName,
                PublishedAt = DateTime.UtcNow,
                Tags = objectTags,
                OwnerLoginId = login.LoginId,
            };

            if (objectDto.Type == TransactionType.Free)
            {
                @object.ObjectFreeProperties = new ObjectFreeProperties
                {
                    OfferedFreeAtUtc = DateTime.UtcNow,
                };
                @object.CurrentTransactionType = TransactionType.Free;
            }
            else if (objectDto.Type == TransactionType.Lending)
            {
                @object.ObjectLoanProperties = new ObjectLoanProperties
                {

                };
                @object.CurrentTransactionType = TransactionType.Lending;
            }

            _objectRepo.Add(@object);
            _objectRepo.SaveChanges();
            return new CommandResult<OfferedObject>(@object);
        }
    }
}
