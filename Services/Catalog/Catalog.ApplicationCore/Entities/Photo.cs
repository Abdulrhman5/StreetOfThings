using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public abstract class Photo : IEntity<int>
    {
        public int PhotoId { get; set; }

        public string FilePath { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime AddedAtUtc { get; set; }

        public int Id => PhotoId;
    }
}
