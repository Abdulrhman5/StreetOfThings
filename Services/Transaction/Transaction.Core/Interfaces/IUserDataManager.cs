using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Dtos;
using Transaction.Domain.Entities;

namespace Transaction.Core.Interfaces
{
    public interface IUserDataManager
    {
        Task<(Login Login, User User)> AddCurrentUserIfNeeded();
        Task<(Login Login, User User)> AddUserIfNotExisted(Guid tokenId, string originUserId, string accessToken);

        Task<User> AddUserIfNeeded(string userId);

        LoginDataDto GetCurrentUser();

    }
}
