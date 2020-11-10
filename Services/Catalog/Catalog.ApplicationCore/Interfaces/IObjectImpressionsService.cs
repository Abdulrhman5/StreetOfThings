using Catalog.ApplicationCore.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    interface IObjectImpressionsService
    {
        Task AddImpressions(List<int> objectsIds);
        Task AddImpressions(List<ObjectDto> objects);
    }
}