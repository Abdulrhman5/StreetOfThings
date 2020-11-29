using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Domain.Entities
{
    public class OfferedObject : BaseEntity<int>
    {
        public int OfferedObjectId { get; set; }

        public int OriginalObjectId { get; set; }

        public bool ShouldReturn { get; set; }

        public float? HourlyCharge { get; set; }
            
        public User OwnerUser { get; set; }

        public Guid OwnerUserId { get; set; }

        public List<ObjectRegistration> Registrations { get; set; }

        public override int Id => OfferedObjectId;
    }
}
