using Catalog.DataAccessLayer;
using Catalog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Catalog.ApplicationLogic.TypeQueries
{
    public class TagsGetter
    {
        private IRepository<int, Tag> _tagsRepo;

        public TagsGetter(IRepository<int,Tag> tagRepo)
        {
            _tagsRepo = tagRepo;
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
    }
}
