using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transaction.Models;

namespace Transaction.Service.Infrastructure
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
            
            modelBuilder.Entity<ObjectReceiving>()
                .HasOne(receiving => receiving.ObjectReturning)
                .WithOne(returning => returning.ObjectReceiving)
                .HasForeignKey<ObjectReturning>(returning => returning.ObjectReceivingId);

            modelBuilder.Entity<Login>()
                .HasMany(login => login.RegistrationRecepiants)
                .WithOne(registration => registration.RecipientLogin)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Login>()
                .HasMany(login => login.ObjectReceivingRecepiants)
                .WithOne(registration => registration.RecipientLogin)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Login>()
                .HasMany(login => login.ObjectReceivingGivers)
                .WithOne(registration => registration.GiverLogin)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Login>()
                .HasMany(login => login.ObjectReturningLoaners)
                .WithOne(returning => returning.LoanerLogin)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Login>()
                .HasMany(login => login.ObjectReturningLoanees)
                .WithOne(returning => returning.LoaneeLogin)
                .OnDelete(DeleteBehavior.NoAction);
        }


        public virtual IDbContextTransactionWrapper BeginTransaction()
        {
            return new DbContextTransactionWrapper(Database.BeginTransaction());
        }

        public virtual async Task<IDbContextTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return new DbContextTransactionWrapper(await Database.BeginTransactionAsync());
        }
    }
}
