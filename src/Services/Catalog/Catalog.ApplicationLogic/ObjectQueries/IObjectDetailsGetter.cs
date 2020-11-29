using CommonLibrary;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectQueries
{
    public interface IObjectDetailsGetter
    {
        Task<CommandResult<ObjectDetailsDto>> GetObjectDetails(int objectId);
    }
}