using Catalog.Models;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.Infrastructure
{
    interface IUserDataManager
    {
        Task<(Login, User)> AddCurrentUserIfNeeded();
        Task<(Login, User)> AddUserIfNotExisted(string tokenId, string originUserId, string accessToken);
    }
}