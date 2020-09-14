using Catalog.Models;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    public interface IUserDataManager
    {
        Task<(Login, User)> AddCurrentUserIfNeeded();
        Task<(Login, User)> AddUserIfNotExisted(string tokenId, string originUserId, string accessToken);

        Task<User> AddUserIfNeeded(string userId);
    }
}