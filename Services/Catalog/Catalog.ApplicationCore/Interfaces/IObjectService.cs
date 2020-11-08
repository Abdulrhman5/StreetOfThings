using Catalog.ApplicationCore.Dtos;
using Catalog.ApplicationCore.Entities;
using Catalog.ApplicationCore.Services;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    interface IObjectService
    {
        Task<CommandResult<OfferedObject>> AddObject(AddObjectDto objectDto);
        Task<CommandResult> AuthorizedDelete(DeleteObjectDto objectDto);
        Task<CommandResult> DeleteObject(DeleteObjectDto objectDto);
    }
}