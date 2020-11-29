using System.Linq;
using Catalog.ApplicationLogic.TypeQueries;
using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using HostingHelpers;
using System.Threading.Tasks;
using Catalog.ApplicationLogic.Infrastructure;

namespace Catalog.ApplicationLogic.TagCommands
{
    public class TagAdder
    {
        private IRepository<int, Tag> _tagsRepo;

        private IImageSaver _imageSaver;

        private IPhotoUrlConstructor _photoUrlConstructor;
        public TagAdder(IRepository<int, Tag> tagsRepo, IImageSaver imageSaver, IPhotoUrlConstructor photoUrlConstructor)
        {
            _tagsRepo = tagsRepo;
            _imageSaver = imageSaver;
            _photoUrlConstructor = photoUrlConstructor;
        }

        public async Task<CommandResult<TagDto>> AddTag(AddTagDto tag)
        {
            if(tag is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.NULL",
                    Message = "Please send a valid data",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<TagDto>();
            }

            if (tag.TagName.IsNullOrEmpty())
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.NAME.EMPTY",
                    Message = "Please send a valid Tag name",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<TagDto>();
            }       
            
            if (tag.Photo is null)
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.PHOTO.EMPTY",
                    Message = "Please send a valid photo",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<TagDto>();
            }

            var existedTags = from t in _tagsRepo.Table
                              where t.Name == tag.TagName
                              select t;

            if (existedTags.Any())
            {
                return new ErrorMessage
                {
                    ErrorCode = "CATALOG.TAG.ADD.ALREADY.EXISTED",
                    Message = "The tag you are trying to add is already existed.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }.ToCommand<TagDto>();
            }

            var imageSavingResult = await _imageSaver.SaveImage(tag.Photo);

            if (!imageSavingResult.IsSuccessful)
            {
                return imageSavingResult.Error.ToCommand<TagDto>();
            }

            var tagModel = new Tag
            {
                Photo = new TagPhoto
                {
                    FilePath = imageSavingResult.Result.Path,
                    AdditionalInformation = QueryString.Create(new Dictionary<string, string>
                    {
                        { "Name", imageSavingResult.Result.Name.ToString() },
                        { "Version", "1" }
                    }).ToUriComponent(),
                    AddedAtUtc = DateTime.UtcNow,
                },
                Name = tag.TagName,
                Description = tag.Discreption,
            };

            _tagsRepo.Add(tagModel);
            await _tagsRepo.SaveChangesAsync();

            return new CommandResult<TagDto>(
                new TagDto
                {
                    Name = tagModel.Name,
                    Id = tagModel.Id,
                    PhotoUrl = _photoUrlConstructor.Construct(tagModel.Photo)
                });
        }
    }

    public class AddTagDto
    {
        public string TagName { get; set; }

        public string Discreption { get; set; }
        public IFormFile Photo { get; set; }
    }
}
