using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Transaction.DataAccessLayer
{
    public class DalUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;
            container.RegisterType(typeof(IRepository<,>),typeof(GenericRepository<,>));
        }
    }
}
