using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public class ObjectPhoto : Photo
    {
        public int ObjectId { get; set; }
        public OfferedObject Object { get; set; }
    }
}
