using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Models;

namespace Catalog.DataAccessLayer
{
    public class CatalogContext : DbContext
    {
        public DbSet<OfferedObject> Objects { get; set; }

        public DbSet<ObjectTag> ObjectTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ObjectLoanProperties> ObjectesLoanProperties { get; set; }

        public DbSet<ObjectLoan> ObjectLoans { get; set; }

        public DbSet<Login> Logins { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ObjectImpression> ObjectImpressions { get; set; }
        public CatalogContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObjectLoanProperties>(eb =>
            {
                eb.HasKey(op => op.ObjectId);
                eb.HasOne(op => op.Object)
                    .WithOne(o => o.ObjectLoanProperties)
                    .HasForeignKey<ObjectLoanProperties>(op => op.ObjectId);
            });

            modelBuilder.Entity<ObjectFreeProperties>(eb =>
            {
                eb.HasKey(op => op.ObjectId);
                eb.HasOne(op => op.Object)
                    .WithOne(o => o.ObjectFreeProperties)
                    .HasForeignKey<ObjectFreeProperties>(op => op.ObjectId);
            });

            modelBuilder.Entity<ObjectTag>().HasKey(ot => new { ot.ObjectId, ot.TagId });

            modelBuilder.Entity<ObjectImpression>().HasKey(op => new { op.ObjectId, op.LoginId, op.ViewedAtUtc });
            modelBuilder.Entity<ObjectView>().HasKey(op => new { op.ObjectId, op.LoginId, op.ViewedAtUtc });

            modelBuilder.Entity<Login>().HasMany(u => u.OwnedObjects).WithOne(ob => ob.OwnerLogin).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TagPhoto>(eb =>
            {
                eb.HasKey(tagPhoto => tagPhoto.TagId);
                eb.HasOne(tagPhoto => tagPhoto.Tag)
                    .WithOne(tag => tag.Photo)
                    .HasForeignKey<TagPhoto>(tagPhoto => tagPhoto.TagId);
            });
        }
    }
}
