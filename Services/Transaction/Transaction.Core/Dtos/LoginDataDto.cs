using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Core.Dtos
{
    public class LoginDataDto
    {
        public string TokenId { get; set; }

        public string UserId { get; set; }

        public string AccessToken { get; set; }
    }
}
