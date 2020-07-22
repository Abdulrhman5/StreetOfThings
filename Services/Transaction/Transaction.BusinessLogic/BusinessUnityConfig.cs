using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.BusinessLogic.ReceivingCommands;
using Transaction.BusinessLogic.RegistrationCommands;
using Transaction.DataAccessLayer;
using Unity;

namespace Transaction.BusinessLogic
{
    public class BusinessUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container { get; private set; }
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<IRemotlyObjectGetter, RemoteObjectGetter>();
            Container.RegisterType<INewRegistrationAdder, NewRegistrationAdder>();
            Container.RegisterType<CurrentUserCredentialsGetter>();
            Container.RegisterType<UserDataGetter>();
            Container.RegisterType<UserDataManager>();
            Container.RegisterType<ObjectDataManager>();

            Container.RegisterType<IAlphaNumericStringGenerator, RngAlphaNumericStringGenerator>();
            Container.RegisterType<ITransactionTokenManager, TransactionTokenManager>();
            Container.RegisterType<IObjectReceivingAdder, ObjectReceivingAdder>();

            Container.RegisterType(typeof(OwnershipAuthorization<,>));
            new DalUnityConfig().ConfigUnity(Container);
                    
        }
    }
}
