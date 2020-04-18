using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class AppUser : IdentityUser, IEntity<string>
    {
        public DateTime CreatedAt { get; set; }

        public string NormalizedName { get; set; }

        public Gender Gender { get; set; }

        public List<Login> Logins { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
