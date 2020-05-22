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
                                     where objectDto.Tags.Any(tt => tt.EqualsIC(t.Name))
                                     select t).ToList();

            var tagsNamesToBeAdded = from t in objectDto.Tags
                                where alreadyExistedTags.All(tt => tt.Name.EqualsIC(t))
                                select t;

            var addedTags = new List<Tag>();
            foreach(var tagName in tagsNamesToBeAdded)
            {
                var addingResult = _tagRepo.Add(new Tag
                {
                    Name = tagName,
                });
                addedTags.Add(addingResult);
            }

            await _tagRepo.SaveChangesAsync();
            addedTags.AddRange(alreadyExistedTags);
            var objectTags = addedTags.Select(t => new ObjectTag
            {
                TagId = t.TagId
            }).ToList();


            var @object = new OfferedObject
            {
                Description = objectDto.Description,
                Name = objectDto.ObjectName,
                PublishedAt = DateTime.UtcNow,
                Tags = objectTags,

            };
            return null;

        }
    }
}
