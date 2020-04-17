using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Location : IEntity<Guid>
    {
        public Guid LocationId { get; set; }

        public Point Point { get; set; }

        public DateTime CreatedAt { get; set; }

        public AppUser User { get; set; }

        public string UserId { get; set; }

        public Guid Id => LocationId;
    }
}
