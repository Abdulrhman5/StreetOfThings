using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    public class ObjectLoan
    {
        public Guid ObjectLoanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public float? Rating { get; set; }

        public Guid LoginId { get; set; }

        public Login Login { get; set; }

        // One to many relationship
        public LendingProperties LendingProperties { get; set; }
    }
}
