﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Entities
{
    public class Tag :IEntity<int>
    {
        public int TagId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TagStatus TagStatus { get; set; }
        public List<ObjectTag> Objects { get; set; }

        public TagPhoto Photo { get; set; }
        public int Id => TagId;
    }

    public enum TagStatus
    {
        Ok,
        Deleted
    }
}
