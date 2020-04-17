using ApplicationLogic.AppUserCommands;
using ApplicationLogic.AppUserQueries;
using CommonLibrary;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace ApplicationLogic
{
    public class ApplicationLogicUnityConfiguration : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<IRegisterUserValidator, RegisterUserValidator>();
            Container.RegisterType<IUserRegisterer, RegisterUser>();

            Container.RegisterType<IUserService, UserGetter>();

            new DalUnityConfiguration().ConfigUnity(container);
        }
    }
}
