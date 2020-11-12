using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Service.Dtos
{
    public class GenerateReturnTokenDto
    {
        public string RegistrationId { get; set; }
    }

    public class GenerateReturnTokenResultDto
    {
        public string ReturnToken { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

    }
}
