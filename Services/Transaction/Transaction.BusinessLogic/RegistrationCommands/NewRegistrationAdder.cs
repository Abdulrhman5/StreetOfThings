using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.BusinessLogic.Infrastructure;
using Transaction.DataAccessLayer;
using Transaction.Models;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public class NewRegistrationAdder : INewRegistrationAdder
    {
        UserDataManager _userDataManager;

        private readonly IRepository<ulong, ObjectRegistration> _registrationsRepo;

        private readonly ObjectDataManager _objectDataManager;



        public async Task<CommandResult<ObjectRegistrationDto>> AddNewRegistrationAsync(AddNewRegistrationDto newRegistrationDto)
        {
            var user = _userDataManager.AddCurrentUserIfNeeded();
            if(user == null)
            {
                return new CommandResult<ObjectRegistrationDto>(new ErrorMessage()
                {
                    ErrorCode = "TRANSACTION.OBJECT.RESERVE.NOT.AUTHORIZED",
                    Message = "You are not authorized to do this operation",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                });
            }
        }
    }
}
