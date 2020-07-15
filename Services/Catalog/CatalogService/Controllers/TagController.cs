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

        public TagController(TagsGetter tagGetter)
        {
            _tagGetter = tagGetter;
        }

        [Route("list")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTags()
        {
            var result = await _tagGetter.GetTags();
            return Ok(result);
        }
    }
}
