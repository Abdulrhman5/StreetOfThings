using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLogic.UserConfirmations
{
    public class ConfirmEmailDto
    {
        public string Email { get; set; }

        public string ConfirmationCode { get; set; }
    }
}
