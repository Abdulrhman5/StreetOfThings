using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IObjectViewsService
    {
        public Task AddView(int objectId);
    }
}
