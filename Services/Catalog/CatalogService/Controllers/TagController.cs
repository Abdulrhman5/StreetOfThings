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
        public TagController(TagsGetter tagGetter, TagAdder tagAdder)
        {
            _tagGetter = tagGetter;
            _tagAdder = tagAdder;
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
    }
}
