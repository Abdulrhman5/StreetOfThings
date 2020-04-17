using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Imei : IEntity<Guid>
    {
        public Guid ImeiId { get; set; }

        public string ImeiString { get; set; }

        public DateTime CreatedAt { get; set; }

        public AppUser User { get; set; }

        public string UserId { get; set; }

        public Guid Id => ImeiId;
    }
}
