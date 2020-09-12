using Catalog.DataAccessLayer;
using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Catalog.ApplicationLogic.Infrastructure;

namespace Catalog.ApplicationLogic.TypeQueries
{
    public class TagsGetter
    {
        private IRepository<int, Tag> _tagsRepo;

        private IPhotoUrlConstructor _urlConstructor;
        public TagsGetter(IRepository<int, Tag> tagRepo, IPhotoUrlConstructor urlConstructor)
        {
            _tagsRepo = tagRepo;
            _urlConstructor = urlConstructor;
        }

        public async Task<List<TagDto>> GetTags()
        {
            var tags = from t in _tagsRepo.Table
                       select new TagDto
                       {
                           Id = t.TagId,
                           Name = t.Name
                       };

            return await tags.ToListAsync();
        }

        public async Task<TagListDto> GetAdminTags()
        {
            var tags = await (from t in _tagsRepo.Table
                       let objectCount = t.Objects.Count
                       orderby objectCount
                       select new AdminTagDto
                       {
                           Id = t.TagId,
                           Name = t.Name,
                           PhotoUrl = _urlConstructor.Construct(t.Photo),
                           UsedCount = objectCount
                       }).ToListAsync();

            var listDto = new TagListDto
            {
                LeastUsed = tags.FirstOrDefault(),
                TopUsed = tags.LastOrDefault(),
                TagCount = tags.Count,
                Tags = tags
            };

            return listDto;
        }
    }

    public class TagListDto
    {
        public TagDto TopUsed { get; set; }

        public TagDto LeastUsed { get; set; }

        public int TagCount { get; set; }

        public List<AdminTagDto> Tags { get; set; }
    }

    public class AdminTagDto : TagDto
    {
        public int UsedCount { get; set; }
    }
}
