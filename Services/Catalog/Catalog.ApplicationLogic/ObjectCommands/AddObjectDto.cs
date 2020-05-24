using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public class AddObjectDto
    {
        public string ObjectName { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type { get; set; }

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ObjectType
    {
        Lending,
        Free
    }
}
