using CommonLibrary;
using Infrastructure.Emails;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Infrastructure
{
    public class InfrastructureUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<IEmailSender, EmailSender>();
        }

    }
}
