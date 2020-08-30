using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Models
{
    public class ObjectLike : IEntity<Guid>
    {
        public Guid ObjectLikeId { get; set; }

        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public DateTime LikedAtUtc { get; set; }

        public Guid LoginId { get; set; }
        public Login Login { get; set; }

        public Guid Id => ObjectLikeId;
    }
}
