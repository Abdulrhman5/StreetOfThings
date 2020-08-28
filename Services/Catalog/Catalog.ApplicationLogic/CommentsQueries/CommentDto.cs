using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.CommentsQueries
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }

        public int ObjectId { get; set; }

        public string Comment { get; set; }

        public string UserId { get; set; }

        public DateTime CommentedAtUtc { get; set; }
    }
}
