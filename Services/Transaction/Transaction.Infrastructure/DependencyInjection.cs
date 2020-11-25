using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Infrastructure.Data;

namespace Transaction.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient(typeof(IRepository<,>), typeof(GenericRepository<,>));
			return services;
		}
	}

}
