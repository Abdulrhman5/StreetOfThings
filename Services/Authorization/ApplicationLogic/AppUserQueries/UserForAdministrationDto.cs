using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLogic.AppUserQueries
{
    public class UserForAdministrationDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public int AccessFeildCount { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
