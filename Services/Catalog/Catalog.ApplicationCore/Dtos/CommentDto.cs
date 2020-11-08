using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Dtos
{
    public class CommentListDto
    {
        public int CommentsCount { get; set; }

        public List<CommentDto> Comments { get; set; }
    }
    public class CommentDto
    {
        public Guid CommentId { get; set; }

        public int ObjectId { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public DateTime CommentedAtUtc { get; set; }
    }
}
