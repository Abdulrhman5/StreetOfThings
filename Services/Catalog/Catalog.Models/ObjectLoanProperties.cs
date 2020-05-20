using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    [Table("ObjectsLoanProperties")]
    public class ObjectLoanProperties
    {
        public Guid LendingPropertiesId { get; set; }

        public List<ObjectLoan> ObjectLoans { get; set; }

        public OfferedObject Object { get; set; }
    }
}
