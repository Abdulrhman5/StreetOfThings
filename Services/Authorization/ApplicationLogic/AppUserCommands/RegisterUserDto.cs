using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLogic.AppUserCommands
{
    public class RegisterUserDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }
    }
}
