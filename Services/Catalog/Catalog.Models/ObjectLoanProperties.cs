using CommonLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Models
{
    [Table("ObjectsLoanProperties")]
    public class ObjectLoanProperties : IEntity<int>
    {
        [Key,ForeignKey(nameof(OfferedObject))]
        public int ObjectId { get; set; }

        public OfferedObject Object { get; set; }


        public List<ObjectLoan> ObjectLoans { get; set; }

        public int Id => ObjectId;
    }
}
