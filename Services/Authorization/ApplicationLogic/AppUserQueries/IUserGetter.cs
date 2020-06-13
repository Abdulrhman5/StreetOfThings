using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    public interface IUserGetter
    {
        IEnumerable<UserDto> GetUserByIds(List<string> usersId);
        IEnumerable<UserForAdministrationDto> GetUsers();
    }
}
