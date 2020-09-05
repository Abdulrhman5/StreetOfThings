using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway.Services.ObjectCommentServices
{
    public class UpstreamCommentListDto
    {
        public int CommentsCount { get; set; }

        public List<UpstreamCommentDto> Comments { get; set; }
    }

    public class UpstreamCommentDto
    {
        public Guid CommentId { get; set; }

        public int ObjectId { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public DateTime CommentedAtUtc { get; set; }
    }
    public class DownstreamCommentListDto
    {
        public int CommentsCount { get; set; }

        public List<DownstreamCommentDto> Comments { get; set; }
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
