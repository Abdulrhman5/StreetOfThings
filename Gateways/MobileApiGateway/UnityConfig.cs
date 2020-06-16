using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace MobileApiGateway
{
    public class UnityConfig : CommonLibrary.IUnityConfigueration
    {
        public void ConfigUnity(IUnityContainer container)
        {
            container.RegisterType<HttpClientHelpers>();
        }
    }
}
