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

        public DateTime PublishedAt { get; set; }

        public DateTime? EndsAt { get; set; }

        public bool IsLending { get; set; }

        public string OwnerId { get; set; }
        public Owner Owner { get; set; }

        public List<ObjectTag> Tags { get; set; }

        public List<User> Users { get; set; }
    }
}
