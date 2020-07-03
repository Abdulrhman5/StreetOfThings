using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Transaction.DataAccessLayer
{
    class DalUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container;
        public void ConfigUnity(IUnityContainer container)
        {
            Container.RegisterType<TransactionContext>();
            Container.RegisterType(typeof(IRepository<,>),typeof(GenericRepository<,>));
        }
    }
}
