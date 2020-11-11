using Catalog.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IImageSaver
    {
        public Task<CommandResult<(string Path, string Name)>> SaveImageAsync(IFormFile file);
    }
}
