using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.RegistrationCommands
{
    public interface INewRegistrationAdder
    {
        public Task<CommandResult<ObjectRegistrationDto>> AddNewRegistrationAsync(AddNewRegistrationDto newRegistrationDto);
    }
}
