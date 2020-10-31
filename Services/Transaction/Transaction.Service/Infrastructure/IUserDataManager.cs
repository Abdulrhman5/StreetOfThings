using System.Threading.Tasks;
using Transaction.Models;

namespace Transaction.Service.Infrastructure
{
    public interface IUserDataManager
    {
        Task<(Login Login, User User)> AddCurrentUserIfNeeded();
        Task<User> AddUserIfNotExisted(string originUserId);
        Task<(Login, User)> AddUserIfNotExisted(string tokenId, string originUserId, string accessToken);
    }
}