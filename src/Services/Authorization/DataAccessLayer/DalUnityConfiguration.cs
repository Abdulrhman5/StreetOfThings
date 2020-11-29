using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace DataAccessLayer
{
    public class DalUnityConfiguration : IUnityConfigueration
    {
        public static IUnityContainer Container { get; set; }

        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            container.RegisterType<AuthorizationContext, AuthorizationContext>();

            Container.RegisterType(typeof(IRepository<,>), typeof(GenericRepository<,>));
        }
    }
}
