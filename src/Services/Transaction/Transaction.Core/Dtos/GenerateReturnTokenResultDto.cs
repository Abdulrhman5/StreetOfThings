using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class GenerateReturnTokenResultDto
    {
        public string ReturnToken { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UseBeforeUtc { get; set; }

    }
}
