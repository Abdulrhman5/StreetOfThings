using System.Collections.Generic;

namespace ApplicationLogic.AppUserQueries
{
    public class UserListDto
    {
        public int AllUserCount { get; set; }

        public int BlockedUsersCount { get; set; }

        public int MaleUsersCount { get; set; }

        public int FemaleUsersCount { get; set; }

        public List<UserForAdministrationDto> Users { get; set; }
    }
}
