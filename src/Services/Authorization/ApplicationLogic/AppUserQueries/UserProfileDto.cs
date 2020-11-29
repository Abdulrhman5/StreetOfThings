using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLogic.AppUserQueries
{
    public class UserProfileDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PictureUrl { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }
    }
}
