using CommonLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HostingHelpers
{
    public interface IImageSaver 
    {
        public Task<CommandResult<SavedImageViewModel>> SaveImage(IFormFile image);
    }
}
