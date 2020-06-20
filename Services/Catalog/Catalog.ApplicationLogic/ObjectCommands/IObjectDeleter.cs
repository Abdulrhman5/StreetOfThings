using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationLogic.ObjectCommands
{
    public interface IObjectDeleter
    {
        public Task<CommandResult> DeleteObject(DeleteObjectDto objectDto);
    }
}
