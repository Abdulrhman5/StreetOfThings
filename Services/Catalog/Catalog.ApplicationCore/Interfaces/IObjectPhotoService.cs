using Catalog.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IObjectPhotoService
    {
        Task<CommandResult> AddPhotoToObject(int objectId, IFormFile image);
    }
}