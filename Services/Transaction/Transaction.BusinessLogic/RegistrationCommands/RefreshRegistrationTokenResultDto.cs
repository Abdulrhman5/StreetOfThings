using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class RefreshRegistrationTokenResultDto
    {
        public string RegistrationToken { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

    }

    public class RefreshRegistrationTokenDto
    {
        public int ObjectId { get; set; }
    }
}
