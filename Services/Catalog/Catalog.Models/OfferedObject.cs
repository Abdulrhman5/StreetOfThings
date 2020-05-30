using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class OfferedObject : IEntity<int>
    {
        public int OfferedObjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime PublishedAt { get; set; }

        public DateTime? EndsAt { get; set; }

        public bool IsLending { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; }

        public List<ObjectTag> Tags { get; set; }

        public ObjectLoanProperties ObjectLoanProperties { get; set; }

        public List<ObjectPhoto> Photos { get; set; }

        public int Id => OfferedObjectId;
    }
}
