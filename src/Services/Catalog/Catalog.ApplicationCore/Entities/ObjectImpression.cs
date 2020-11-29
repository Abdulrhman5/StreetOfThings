using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public class ObjectImpression : IEntity<(int ObjectId, Guid LoginId, DateTime ViewedAtUtc)>
    {
        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public Guid LoginId { get; set; }
        public Login Login { get; set; }

        public DateTime ViewedAtUtc { get; set; }

        public (int ObjectId, Guid LoginId, DateTime ViewedAtUtc) Id => (ObjectId, LoginId, ViewedAtUtc);
    }
}
