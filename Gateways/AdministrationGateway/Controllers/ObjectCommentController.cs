using AdministrationGateway.Services;
using Grpc.Core;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdministrationGateway.Controllers
{
    [Route("api/Catalog/Object/Comment")]
    [Authorize("Admin")]
    public class ObjectCommentController : MyControllerBase
    {
        private CommentAggregator _commentAggregator;

        public ObjectCommentController(CommentAggregator commentAggregator)
        {
            _commentAggregator = commentAggregator;
        }

        [Route("forObject")]
        [HttpGet]
        public async Task<IActionResult> GetObjectComments()
        {
            var result = await _commentAggregator.AggregateComments();
            return StatusCode(result);
        }
    }
}
