using Catalog.ApplicationLogic.TagCommands;
using Catalog.ApplicationLogic.TypeQueries;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/Tag")]
    public class TagController : MyControllerBase
    {
        private TagsGetter _tagGetter;

        private TagAdder _tagAdder;

        private TagDeleter _tagDeleter;
        public TagController(TagsGetter tagGetter, TagAdder tagAdder, TagDeleter tagDeleter)
        {
            _tagGetter = tagGetter;
            _tagAdder = tagAdder;
            _tagDeleter = tagDeleter;
        }

        [Route("list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTags()
        {
            var result = await _tagGetter.GetTags();
            return Ok(result);
        }  
        
        [Route("add")]
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> AddTag(AddTagDto tag)
        {
            var result = await _tagAdder.AddTag(tag);
            return StatusCode(result);
        }

        [Route("admin/list")]
        [HttpGet]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAdminTags()
        {
            var result = await _tagGetter.GetAdminTags();
            return Ok(result);
        }  
        
        [Route("admin/delete")]
        [HttpPost]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteTag([FromBody] DeleteTagDto deleteTag)
        {
            var result = await _tagDeleter.DeleteTag(deleteTag);
            return StatusCode(result, new
            {
                Message = "The tag has been deleted"
            });
        }

    }
}
