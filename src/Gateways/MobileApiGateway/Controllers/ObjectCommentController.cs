using Grpc.Core;
using HostingHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileApiGateway.Services;
using MobileApiGateway.Services.ObjectCommentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApiGateway.Controllers
{
    [Route("api/Catalog/Object/Comment")]
    public class ObjectCommentController : MyControllerBase
    {
        private CommentAggregator _commentAggregator;

        public ObjectCommentController(CommentAggregator commentAggregator)
        {
            _commentAggregator = commentAggregator;
        }

        [Route("forObject")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetObjectComments()
        {
            var result = await _commentAggregator.GetAndAggregateComments();
            return StatusCode(result);
        }
    }
}
