using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Models
{
    public class Object
    {
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ObjectTag> Tags { get; set; }
    }
}
