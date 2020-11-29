using CommonLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Catalog.DataAccessLayer
{
    public class DalUnityConfig : IUnityConfigueration
    {
        public static IUnityContainer Container;
        public void ConfigUnity(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<CatalogContext>();
            Container.RegisterType(typeof(IRepository<,>), typeof(GenericRepository<,>));
        }
    }
}
