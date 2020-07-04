using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class ObjectRegistrationDto
    {
        public int RegistrationId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public int ObjectId { get; set; }

    }
}
