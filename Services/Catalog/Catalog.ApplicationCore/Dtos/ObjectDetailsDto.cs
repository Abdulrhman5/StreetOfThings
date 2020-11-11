using Catalog.ApplicationCore.Entities;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Catalog.ApplicationCore.Dtos
{
    public class ObjectDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CountOfViews { get; set; }

        public int CountOfImpressions { get; set; }

        public float? Rating { get; set; }

        public string OwnerId { get; set; }

        public List<string> Photos { get; set; }

        public List<string> Tags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType Type { get; set; }

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }

        public bool IsLikedByMe { get; set; }

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
