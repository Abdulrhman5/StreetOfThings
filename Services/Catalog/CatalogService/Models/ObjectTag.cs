using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Models
{
    public class ObjectTag
    {
        public int ObjectId { get; set; }
        public Object Object { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
