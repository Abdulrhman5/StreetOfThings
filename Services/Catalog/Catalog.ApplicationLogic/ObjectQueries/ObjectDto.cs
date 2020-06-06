using Catalog.Models;
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

        public float? Rating { get; set; }       

        public string OwnerId { get; set; }

        public List<string> Photos { get; set; }

        public List<string> Tags { get; set; }

        public TransactionType Type { get; set; }
    }
}
