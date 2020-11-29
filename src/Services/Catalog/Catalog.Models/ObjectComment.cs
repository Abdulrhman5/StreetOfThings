using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Models
{
    public class ObjectComment : IEntity<Guid>
    {
        public Guid ObjectCommentId { get; set; }

        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public Guid LoginId { get; set; }
        public Login Login { get; set; }

        public string Comment { get; set; }

        public DateTime AddedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Guid Id => ObjectCommentId;
    }
}
