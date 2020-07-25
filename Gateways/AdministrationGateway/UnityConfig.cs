using System;
using Unity;

namespace AdministrationGateway
{
    internal class UnityConfig
    {
       
        public static IUnityContainer Container { get; private set; }
        internal void ConfigUnity(IUnityContainer container)
        {
            Container = container;
        }
    }
}