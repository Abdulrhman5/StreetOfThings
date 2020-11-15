using System.Threading.Tasks;
using Transaction.Service.Models;

namespace Transaction.Service.Infrastructure
{
    public interface IObjectDataManager
    {
        Task<OfferedObject> GetObjectAsync(int objectId);
    }
}