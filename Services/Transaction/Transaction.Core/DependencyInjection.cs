using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Transaction.Core.Behaviours;
using Transaction.Core.Interfaces;

namespace Transaction.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddTransient(typeof(IOwnershipAuthorization<,>), typeof(OwnershipAuthorization<,>));
            services.AddTransient<IRandomStringGenerator, RngAlphaNumericStringGenerator>();
            services.AddTransient<ITransactionTokenManager, TransactionTokenManager>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
            return services;
        }
    }
}
