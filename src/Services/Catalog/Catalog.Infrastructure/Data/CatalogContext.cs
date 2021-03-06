﻿using Catalog.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : DbContext
    {
        public DbSet<OfferedObject> Objects { get; set; }

        public DbSet<ObjectTag> ObjectTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Login> Logins { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<ObjectImpression> ObjectImpressions { get; set; }

        public DbSet<ObjectPhoto> ObjectPhotos { get; set; }

        public DbSet<TagPhoto> TagPhotos { get; set; }

        public CatalogContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ObjectTag>().HasKey(ot => new { ot.ObjectId, ot.TagId });

            modelBuilder.Entity<ObjectImpression>().HasKey(op => new { op.ObjectId, op.LoginId, op.ViewedAtUtc });
            modelBuilder.Entity<ObjectView>().HasKey(op => new { op.ObjectId, op.LoginId, op.ViewedAtUtc });

            modelBuilder.Entity<Login>().HasMany(u => u.OwnedObjects).WithOne(ob => ob.OwnerLogin).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TagPhoto>(eb =>
            {
                eb.HasKey(tagPhoto => tagPhoto.PhotoId);
                eb.HasOne(tagPhoto => tagPhoto.Tag)
                    .WithOne(tag => tag.Photo)
                    .HasForeignKey<TagPhoto>(tagPhoto => tagPhoto.TagId);
            });

            modelBuilder.Entity<ObjectPhoto>().HasKey(op => op.PhotoId);
        }
    }
}
