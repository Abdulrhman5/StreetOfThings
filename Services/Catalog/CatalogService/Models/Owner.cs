using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Models
{
    public class Owner
    {
        public string OwnerId { get; set; }

        public string OwnerName { get; set; }

        public List<Object> OfferedObjects { get; set; }
    }
}
