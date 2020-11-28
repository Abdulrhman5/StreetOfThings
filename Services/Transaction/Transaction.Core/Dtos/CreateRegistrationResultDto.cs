using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.Service.Dtos
{
    public class CreateRegistrationResultDto
    {
        public Guid RegistrationId { get; set; }

        public int ObjectId { get; set; }

        public TimeSpan? ShouldBeReturnedAfterReceving { get; set; }

        public DateTime RegistrationExpiresAtUtc { get; set; }

        public RegistrationTokenResultDto RegistrationToken { get; set; }
    }

    public class RegistrationTokenResultDto
    {
        public string RegistrationToken { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

    }
}
