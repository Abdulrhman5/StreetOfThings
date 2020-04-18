using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using System;

namespace DataAccessLayer
{
    public class AuthorizationContext : IdentityDbContext<AppUser>
    {
        public AuthorizationContext(DbContextOptions options) : base(options)
        {

        }
    }
}
