using CommonLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Catalog.Models
{
    public class ObjectFreeProperties : IEntity<int>
    {
        [Key, ForeignKey(nameof(OfferedObject))]
        public int ObjectId { get; set; }

        public OfferedObject Object { get; set; }

        public Guid? TakerId { get; set; }

        public User Taker { get; set; }

        public DateTime? TakenAtUtc { get; set; }

        public DateTime OfferedFreeAtUtc { get; set; }

        public int Id => ObjectId;
    }
}
