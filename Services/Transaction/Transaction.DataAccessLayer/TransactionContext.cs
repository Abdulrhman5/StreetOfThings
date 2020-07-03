using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.Models;

namespace Transaction.DataAccessLayer
{
    public class TransactionContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Login> Logins { get; set; }

        public DbSet<ObjectRegistration> ObjectRegistrations { get; set; }

        public DbSet<ObjectReceiving> ObjectReceivings { get; set; }

        public DbSet<ObjectReturning> ObjectReturnings { get; set; }

        public TransactionContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ObjectRegistration>()
                .HasOne(registration => registration.ObjectReceiving)
                .WithOne(receiving => receiving.ObjectRegistration)
                .HasForeignKey<ObjectReceiving>(receiving => receiving.ObjectRegistrationId);
        }
    }
}
