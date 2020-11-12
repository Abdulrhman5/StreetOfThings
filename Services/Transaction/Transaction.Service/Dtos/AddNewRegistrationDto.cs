using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Service.Dtos
{
    public class AddNewRegistrationDto
    {
        public int ObjectId { get; set; }

        public int? ShouldReturnAfter { get; set; }
    }
}
