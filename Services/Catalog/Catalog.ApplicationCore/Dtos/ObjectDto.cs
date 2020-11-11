using Catalog.ApplicationCore.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Catalog.ApplicationCore.Dtos
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
        public double? DistanceInMeters { get; set; }
    }

    public class ObjectsForUserListDto
    {
        public int ReservedObjectsCount { get; set; }

        public int AvailableObjectsCount { get; set; }

        public List<ObjectDto> Objects { get; set; }
    }

    public class ObjectsForAdministrationListDto
    {
        public int FreeObjectsCount { get; set; }

        public int RentingObjectsCount { get; set; }

        public int LendingObjectsCount { get; set; }

        public List<ObjectDto> Objects { get; set; }
    }

}
