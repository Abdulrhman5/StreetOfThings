using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CountOfViews { get; set; }

        public int CountOfImpressions { get; set; }

        public float Rating { get; set; }
    }
}
