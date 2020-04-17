using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.AppUserQueries
{
    public interface IUserService
    {
        IEnumerable<UserForAdministrationDto> GetUsers();
    }
}
