using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public class ObjectPhoto : IEntity<int>
    {
        public int ObjectPhotoId { get; set; }

        public string FilePath { get; set; }

        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime AddedAtUtc { get; set; }

        public int Id => ObjectPhotoId;
    }
}
