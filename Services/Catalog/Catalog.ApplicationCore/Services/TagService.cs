using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Catalog.ApplicationCore.Services
{
    public class TagService : ITagService
    {
        private IRepository<int, Tag> _tagsRepo;

        private ILogger<TagService> _logger;
        private IImageSaver _imageSaver;
        public TagService(IRepository<int, Tag> tagsRepo, ILogger<TagService> logger, IImageSaver imageSaver)
        {
            _tagsRepo = tagsRepo;
            _logger = logger;
            _imageSaver = imageSaver;
        }

        public async Task<CommandResult<Tag>> AddTag(AddTagDto tag)
        {
            if(tag is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<Tag>();
            }

            if (tag.TagName.IsNullOrEmpty())
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.NAME.EMPTY",
                    Message = "Please send a valid Tag name",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<Tag>();
            }       
            

            var existedTags = from t in _tagsRepo.Table
                              where t.Name == tag.TagName && t.TagStatus == TagStatus.Ok
                              select t;

            if (existedTags.Any())
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.ALREADY.EXISTED",
                    Message = "The tag you are trying to add is already existed.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<Tag>();
            }


            var imageSavingResult = await _imageSaver.SaveImageAsync(tag.Photo);
            if (!imageSavingResult.IsSuccessful)
            {
                return new ErrorMessage
                {
                    ErrorCode = imageSavingResult.Error.ErrorCode,
                    Message = imageSavingResult.Error.Message,
                    StatusCode = imageSavingResult.Error.StatusCode
                }.ToCommand<Tag>();
            }


            var tagModel = new Tag
            {
                Name = tag.TagName,
                Description = tag.Discreption,
                Photo = new TagPhoto
                {
                    FilePath = imageSavingResult.Result.Path,
                    AdditionalInformation = QueryString.Create(new Dictionary<string, string>
                    {
                        { "Name", imageSavingResult.Result.Name.ToString() },
                        { "Version", "1" }
                    }).ToUriComponent(),
                    AddedAtUtc = DateTime.UtcNow,
                }
            };

            _tagsRepo.Add(tagModel);
            await _tagsRepo.SaveChangesAsync();

            return new CommandResult<Tag>(tagModel);
        }

        public async Task<CommandResult> DeleteTag(DeleteTagDto deleteTagDto)
        {
            if (deleteTagDto == null || deleteTagDto.TagId == 0)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.NULL",
                    Message = "Please send valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var tag = _tagsRepo.Table.SingleOrDefault(t => t.TagId == deleteTagDto.TagId);
            if (tag is null)
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.NOTFOUND",
                    Message = "The tag you are trying to delete does not exists",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            try
            {
                tag.TagStatus = TagStatus.Deleted;
                await _tagsRepo.SaveChangesAsync();
                return new CommandResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There were an error while deleting a tag {tagId}", deleteTagDto.TagId);
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.DELETE.ERROR",
                    Message = "There were an error while executing your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }

    }
}
