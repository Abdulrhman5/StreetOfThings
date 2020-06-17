using AuthorizationService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileApiGateway.Services
{
    public class UpstreamObjectDto
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

    public class DownstreamObjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CountOfViews { get; set; }

        public int CountOfImpressions { get; set; }

        public float? Rating { get; set; }

        public List<string> Photos { get; set; }

        public List<string> Tags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType Type { get; set; }

        public UserDto Owner { get; set; }
        
    }
    public enum TransactionType
    {
        Lending,
        Free
    }

}
