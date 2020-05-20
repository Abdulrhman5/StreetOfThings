using Catalog.DataAccessLayer;
using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    class ObjectAdder : IObjectAdder
    {
        public Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto)
        {
            throw new NotImplementedException();
        }
    }
}
