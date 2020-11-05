using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public class TagPhoto : IEntity<int>
    {
        public string FilePath { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime AddedAtUtc { get; set; }

        public int Id => TagId;
    }
}
