using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public class ObjectsForAdministrationListDto
    {
        public int FreeObjectsCount { get; set; }

        public int RentingObjectsCount { get; set; }

        public int LendingObjectsCount { get; set; }

        public List<ObjectDto> Objects { get; set; }
    }
}
