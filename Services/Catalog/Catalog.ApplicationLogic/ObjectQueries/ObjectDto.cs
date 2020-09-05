using Catalog.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectDto
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
    }

    public class ObjectDtoV1_1 : ObjectDto
    {
        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }

        public bool IsLikedByMe { get; set; }
    }

}
