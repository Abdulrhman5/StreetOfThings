using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationCore.Entities
{
    public class TagPhoto : Photo
    {
        public Tag Tag { get; set; }
        public int TagId { get; set; }

    }
}
