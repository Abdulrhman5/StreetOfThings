using Catalog.ApplicationLogic.ObjectQueries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public interface IObjectImpressionsManager
    {
        Task AddImpressions(List<int> objectsIds);
        Task AddImpressions(List<ObjectDto> objects);
    }
}