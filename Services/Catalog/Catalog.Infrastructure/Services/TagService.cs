using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Extensions;
using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using HostingHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class TagService
    {
        private ITagService _tagService;

        private IImageSaver _imageSaver;

        private IRepository<int, TagPhoto> _tagPhotoRepo;

        public TagService(ITagService tagService, IImageSaver imageSaver, IRepository<int, TagPhoto> tagPhotoRepo)
        {
            _tagService = tagService;
            _imageSaver = imageSaver;
            _tagPhotoRepo = tagPhotoRepo;
        }

        public async Task<CommandResult<Tag>> AddTagWithPhoto(AddTagWithPhotoDto tagDto)
        {
            var savingTagResult = await _tagService.AddTag(tagDto);
            if (!savingTagResult.IsSuccessful)
            {
                return savingTagResult;
            }

            var imageSavingResult = await _imageSaver.SaveImage(tagDto.Photo);
            if (!imageSavingResult.IsSuccessful)
            {
                return new ErrorMessage
                {
                    ErrorCode = imageSavingResult.Error.ErrorCode,
                    Message = imageSavingResult.Error.Message,
                    StatusCode = imageSavingResult.Error.StatusCode
                }.ToCommand<Tag>();
            }

            var photo = new TagPhoto
            {
                FilePath = imageSavingResult.Result.Path,
                AdditionalInformation = QueryString.Create(new Dictionary<string, string>
                    {
                        { "Name", imageSavingResult.Result.Name.ToString() },
                        { "Version", "1" }
                    }).ToUriComponent(),
                AddedAtUtc = DateTime.UtcNow,
                TagId = savingTagResult.Result.TagId
            };

            _tagPhotoRepo.Add(photo);
            await _tagPhotoRepo.SaveChangesAsync();
            savingTagResult.Result.Photo = photo;
            return savingTagResult;
        }
    }

    public class AddTagWithPhotoDto : AddTagDto
    {
        public IFormFile Photo { get; set; }
    }
}
