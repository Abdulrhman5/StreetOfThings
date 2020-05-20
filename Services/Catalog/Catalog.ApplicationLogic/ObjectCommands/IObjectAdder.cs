using Catalog.Models;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public interface IObjectAdder
    {
        Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto);
    }
}
