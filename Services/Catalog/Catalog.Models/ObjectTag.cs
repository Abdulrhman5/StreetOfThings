using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class ObjectTag
    {
        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
