using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class AddNewRegistrationDto
    {
        public int ObjectId { get; set; }

        public int? ShouldReturnAfter { get; set; }
    }
}
