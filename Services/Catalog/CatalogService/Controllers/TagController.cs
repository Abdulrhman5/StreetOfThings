using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/Tag")]
    public class TagController : MyControllerBase
    {
        private ITagService _tagService;
        private IRepository<int, Tag> _tagsRepo;

        private IPhotoUrlConstructor _urlConstructor;

            [Route("list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTags()
        {
            var tags = from t in _tagsRepo.Table
                       where t.TagStatus == TagStatus.Ok
                       select new
                       {
                           Id = t.TagId,
                           Name = t.Name
                       };

            return Ok( await tags.ToListAsync());
        }  
        
        [Route("add")]
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> AddTag(AddTagDto tag)
        {
            var result = await _tagService.AddTag(tag);
            return StatusCode(result);
        }

        [Route("admin/list")]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAdminTags()
        {
            var tags = await (from t in _tagsRepo.Table
                              where t.TagStatus == TagStatus.Ok
                              let objectCount = t.Objects.Count
                              orderby objectCount
                              select new 
                              {
                                  Id = t.TagId,
                                  Name = t.Name,
                                  PhotoUrl = _urlConstructor.Construct(t.Photo),
                                  UsedCount = objectCount
                              }).ToListAsync();

            var listDto = new 
            {
                LeastUsed = tags.FirstOrDefault(),
                TopUsed = tags.LastOrDefault(),
                TagCount = tags.Count,
                Tags = tags
            };

            return Ok(listDto);
        }  
        
        [Route("admin/delete")]
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteTag([FromBody] DeleteTagDto deleteTag)
        {
            var result = await _tagService.DeleteTag(deleteTag);
            return StatusCode(result, new
            {
                Message = "The tag has been deleted"
            });
        }

    }
}
