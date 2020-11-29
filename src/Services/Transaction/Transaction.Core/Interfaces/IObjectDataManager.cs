using System.Threading.Tasks;
using Transaction.Domain.Entities;

namespace Transaction.Core.Interfaces
{
    public interface IObjectDataManager
    {
        Task<OfferedObject> GetObjectAsync(int objectId);
    }
}