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

        public TransactionType CurrentTransactionType { get; set; } 

        public ObjectStatus ObjectStatus { get; set; }
            
        public Guid OwnerLoginId { get; set; }
        public Login OwnerLogin { get; set; }

        public List<ObjectTag> Tags { get; set; }

        public ObjectLoanProperties ObjectLoanProperties { get; set; }

        public ObjectFreeProperties ObjectFreeProperties { get; set; }

        public List<ObjectPhoto> Photos { get; set; }

        public List<ObjectImpression> Impressions { get; set; }

        public List<ObjectComment> Comments { get; set; }
        public int Id => OfferedObjectId;
    }

    public enum TransactionType
    {
        Lending,
        Free
    }

    public enum ObjectStatus
    {
        Available,
        Deleted,
    }
}
