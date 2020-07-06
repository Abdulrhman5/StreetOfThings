using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class ObjectRegistrationDto
    {
        public ulong RegistrationId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
    }
}
