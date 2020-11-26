using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Infrastructure.Data;
using Transaction.Infrastructure.Services;

namespace Transaction.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient(typeof(IRepository<,>), typeof(GenericRepository<,>));
			services.AddTransient<IQueryableHelper, QueryableHelper>();
			services.AddTransient<IUserDataManager, UserDataManager>();
			services.AddTransient<IRemotlyObjectGetter, RemoteObjectGetter>();
			services.AddTransient<IObjectDataManager, ObjectDataManager>();

			return services;
		}
	}

}
