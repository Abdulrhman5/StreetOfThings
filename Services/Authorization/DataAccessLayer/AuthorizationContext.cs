using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using System;

namespace DataAccessLayer
{
    public class AuthorizationContext : IdentityDbContext<AppUser>
    {
        public DbSet<ConfirmationToken> Confirmations { get; set; }

        public AuthorizationContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ConfirmationToken>().HasIndex(ct => ct.ConfirmationCode);
            
        }
    }
}
