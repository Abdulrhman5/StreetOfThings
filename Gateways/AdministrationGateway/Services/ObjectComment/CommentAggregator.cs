using CommonLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdministrationGateway.Services
{
    public class CommentAggregator
    {
        private HttpClient _httpClient;

        private HttpContext _httpContext;

        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private ILogger<ObjectAggregator> _logger;

        private UserService _userService;

        public CommentAggregator(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, HttpClientHelpers responseProcessor, IConfiguration configs, ILogger<ObjectAggregator> logger, UserService userService)
        {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
            _responseProcessor = responseProcessor;
            _configs = configs;
            _logger = logger;
            _userService = userService;
        }

        public async Task<CommandResult<List<DownstreamCommentDto>>> AggregateComments()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Services:Catalog"]}/api/object/comment/forObject", true, true, changeBody: null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var commentsResult = await _responseProcessor.Process<List<UpstreamCommentDto>>(response);
                if (!commentsResult.IsSuccessful)
                {
                    return new CommandResult<List<DownstreamCommentDto>>(commentsResult.Error);
                }
                return new CommandResult<List<DownstreamCommentDto>>(await ReplaceUserIdWithUser(commentsResult.Result));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error When getting list of objects");
                var message = new ErrorMessage
                {
                    ErrorCode = "CATALOG.OBJECT.COMMENT.LIST.ERROR",
                    Message = "there were an error while trying to execute your request",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                };
                return new CommandResult<List<DownstreamCommentDto>>(message);
            }
        }

        private async Task<List<DownstreamCommentDto>> ReplaceUserIdWithUser(List<UpstreamCommentDto> comments)
        {
            var originalUserIds = comments.Select(o => o.UserId).ToList();
            var users = await _userService.GetUsersAsync(originalUserIds);

            var downstreamComments = new List<DownstreamCommentDto>();
            foreach (var comment in comments)
            {
                downstreamComments.Add(new DownstreamCommentDto
                {
                    CommentId=comment.CommentId,
                    Comment = comment.Comment,
                    CommentedAtUtc = comment.CommentedAtUtc,
                    Commenter = users.Find(u => u.Id == comment.UserId),
                     ObjectId = comment.ObjectId
                });
                downstreamComments.RemoveAll(o => o.Commenter is null);
            }

            return downstreamComments;
        }

        private class UpstreamCommentDto
        {
            public Guid CommentId { get; set; }

            public int ObjectId { get; set; }

            public string Comment { get; set; }

            public string UserId { get; set; }

            public DateTime CommentedAtUtc { get; set; }
        }

        public class DownstreamCommentDto
        {
            public Guid CommentId { get; set; }

            public int ObjectId { get; set; }

            public string Comment { get; set; }

            public UserDto Commenter { get; set; }

            public DateTime CommentedAtUtc { get; set; }

        }
    }
}
