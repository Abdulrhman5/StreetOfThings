using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectsForUserListDto
    {
        public int ReservedObjectsCount { get; set; }

        public int AvailableObjectsCount { get; set; }

        public List<ObjectDto> Objects { get; set; }
    }
}
