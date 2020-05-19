using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class LendingProperties
    {
        public Guid LendingPropertiesId { get; set; }

        public List<ObjectLoan> ObjectLoans { get; set; }

        public Object Object { get; set; }
    }
}
