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
        private HttpClientHelpers _responseProcessor;

        private IConfiguration _configs;

        private UserService _userService;

        private IMapper _mapper;
        public CommentAggregator(
            HttpClientHelpers responseProcessor,
            IConfiguration configs,
            UserService userService, IMapper mapper)
        {
            _responseProcessor = responseProcessor;
            _configs = configs;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<CommandResult<UpstreamCommentListDto>> GetComments()
        {
            return await _responseProcessor.CreateAndProcess<UpstreamCommentListDto>(HttpMethod.Get, 
                $"{_configs["Services:Catalog"]}/api/object/comment/forObject", 
                "CATALOG.OBJECT.COMMENT.LIST.ERROR");
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
