using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Transaction.BusinessLogic
{
    public class BusinessUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;
                    
        }
    }
}
