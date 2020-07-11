using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public class OfferedObject : IEntity<int>
    {
        public int OfferedObjectId { get; set; }

        public int OriginalObjectId { get; set; }

        public bool ShouldReturn { get; set; }

        public float? HourlyCharge { get; set; }
            
        public User OwnerUser { get; set; }

        public Guid OwnerUserId { get; set; }

        public int Id => OfferedObjectId;
    }
}
