using Catalog.ApplicationLogic.Infrastructure;
using Catalog.ApplicationLogic.ObjectCommands;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Catalog.ApplicationLogic
{
    public class AppLogicUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }

        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<CurrentUserCredentialsGetter>();
            Container.RegisterType<UserDataManager>();
            Container.RegisterType<UserDataGetter>();
            Container.RegisterType<IObjectAdder, ObjectAdder>();
        }
    }
}
