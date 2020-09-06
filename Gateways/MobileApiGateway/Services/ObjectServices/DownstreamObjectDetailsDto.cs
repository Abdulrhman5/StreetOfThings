using MobileApiGateway.Services.ObjectCommentServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway.Services.ObjectServices
{
    public class DownstreamObjectDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CountOfViews { get; set; }

        public int CountOfImpressions { get; set; }

        public float? Rating { get; set; }

        public UserDto Owner { get; set; }

        public List<string> Photos { get; set; }

        public List<string> Tags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType Type { get; set; }

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }

        public bool IsLikedByMe { get; set; }

        public List<DownstreamCommentDto> Comments { get; set; }

        public LocationDto OwnerLocation { get; set; }
    }

    public class LocationDto
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
