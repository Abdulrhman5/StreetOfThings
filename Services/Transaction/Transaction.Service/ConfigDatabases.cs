using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Transaction.DataAccessLayer;

namespace Transaction.Service
{
    public class ConfigDatabases
    {
        public static void SeedUsersDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var databaseContext = serviceScope.ServiceProvider.GetRequiredService<TransactionContext>();
                databaseContext.Database.Migrate();
            }
        }
    }
}
