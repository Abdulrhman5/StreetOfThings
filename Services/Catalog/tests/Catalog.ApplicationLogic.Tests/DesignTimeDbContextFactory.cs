using Catalog.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.ApplicationLogic.Tests
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        public const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Catalog.Objects.Tests;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public CatalogContext CreateDbContext(string[] args)
        {

            var builder = new DbContextOptionsBuilder<CatalogContext>();
            builder.UseSqlServer(ConnectionString);
            return new CatalogContext(builder.Options);
        }
    }
}
