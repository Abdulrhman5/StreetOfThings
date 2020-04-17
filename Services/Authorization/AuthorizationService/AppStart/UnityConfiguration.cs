using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using DataAccessLayer;
using ApplicationLogic;

namespace AuthorizationService.AppStart
{
    public class UnityConfiguration : IUnityConfigueration
    {
        public static IUnityContainer Container { get; set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            // Register Services

            new ApplicationLogicUnityConfiguration().ConfigUnity(container);
        }
    }
}
