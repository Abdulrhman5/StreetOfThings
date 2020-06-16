using CommonLibrary;
using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ApplicationLogic.AppUserCommands
{
    public class UserControlDeletion
    {
        private IRepository<string, AppUser> _usersRepo;

        private ILogger<UserControlDeletion> _logger;

        public UserControlDeletion(IRepository<string, AppUser> usersRepo, ILogger<UserControlDeletion> logger)
        {
            _usersRepo = usersRepo;
            _logger = logger;
        }

        public async Task<CommandResult> DeleteUser(string userId)
        {

            var user = from u in _usersRepo.Table
                       where u.Id == userId
                       select u;

            if (user is null || !user.Any())
            {
                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CONTROL.DELETION.NOT.FOUND",
                    Message = "The user you are trying to delete does not exist",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }

            try
            {
                _usersRepo.Delete(user.FirstOrDefault());
                await _usersRepo.SaveChangesAsync();

                return new CommandResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There were an error while trying to delete user" + userId);

                return new CommandResult(new ErrorMessage
                {
                    ErrorCode = "USER.CONTROL.DELETION.INTERNAL.ERROR",
                    Message = "There were an error while trying to delete user",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }
        }
    }
}
