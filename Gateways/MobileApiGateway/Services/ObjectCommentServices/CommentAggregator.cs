using AutoMapper;
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

namespace MobileApiGateway.Services.ObjectCommentServices
{
    public class CommentAggregator
    {
        private HttpClient _httpClient;

        private HttpContext _httpContext;

        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private ILogger<CommentAggregator> _logger;

        private UserService _userService;

        private IMapper _mapper;
        public CommentAggregator(HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            HttpClientHelpers responseProcessor,
            IConfiguration configs,
            ILogger<CommentAggregator> logger,
            UserService userService, IMapper mapper)
        {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
            _responseProcessor = responseProcessor;
            _configs = configs;
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<CommandResult<UpstreamCommentListDto>> GetComments()
        {
            var request = await _responseProcessor.CreateAsync(_httpContext, HttpMethod.Get, $"{_configs["Services:Catalog"]}/api/object/comment/forObject", true, true, changeBody: null);
            try
            {
                var response = await _httpClient.SendAsync(request);
                var commentsResult = await _responseProcessor.Process<UpstreamCommentListDto>(response);
                return commentsResult;
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
                return new CommandResult<UpstreamCommentListDto>(message);
            }
        }

        public  async Task<DownstreamCommentListDto> AggregateCommentsWithUsers(UpstreamCommentListDto comments)
        {
            var originalUserIds = comments.Comments.Select(o => o.UserId).ToList();
            var users = await _userService.GetUsersAsync(originalUserIds);

            return await AggregateCommentsWithUsers(comments, users);
        }

        public async Task<DownstreamCommentListDto> AggregateCommentsWithUsers(UpstreamCommentListDto comments, List<UserDto> users)
        {
            var downComments = _mapper.Map<DownstreamCommentListDto>(comments);
            downComments.Comments.ForEach(downComment =>
            {
                var upComment = comments.Comments.FirstOrDefault(c => downComment.CommentId == c.CommentId);
                downComment.Commenter = users.FirstOrDefault(u => u.Id == upComment.UserId);
            });
            return downComments;
        }

        public async Task<CommandResult<DownstreamCommentListDto>> GetAndAggregateComments()
        {
            var commentsResult = await GetComments();
            if (!commentsResult.IsSuccessful)
            {
                return new CommandResult<DownstreamCommentListDto>(commentsResult.Error);
            }
            
            return new CommandResult<DownstreamCommentListDto>(await AggregateCommentsWithUsers(commentsResult.Result));
        }
    }
}
