﻿using Microsoft.EntityFrameworkCore;
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

        public CatalogContext(DbContextOptions options): base(options)
        {

        }
        
    }
}
