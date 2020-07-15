using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class ObjectRegistrationDto
    {
        public Guid RegistrationId { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }
        
        public DateTime RegistrationExpiresAtUtc { get; set; }

        public RegistrationTokenResultDto RegistrationToken { get; set; }
    }
}
