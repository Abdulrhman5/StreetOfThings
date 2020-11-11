using Catalog.ApplicationCore.Interfaces;
using Catalog.ApplicationCore.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IImageSaverAdaptee = HostingHelpers.IImageSaver;
namespace Catalog.Infrastructure
{
    class ImageSaverAdapter : IImageSaver
    {
        private IImageSaverAdaptee _imageAdaptee;

        public ImageSaverAdapter(IImageSaverAdaptee imageAdaptee)
        {
            _imageAdaptee = imageAdaptee;
        }

        public async Task<CommandResult<(string Path, string Name)>> SaveImageAsync(IFormFile file)
        {
            var result = await _imageAdaptee.SaveImage(file);
            if(result.IsSuccessful)
            {
                return new CommandResult<(string Path, string Name)>((result.Result.Path, result.Result.Name.ToString()));
            }
            return new CommandResult<(string Path, string Name)>(new ErrorMessage
            {
                ErrorCode = result.Error.ErrorCode,
                Message = result.Error.Message,
                StatusCode = result.Error.StatusCode
            });
        }
    }
}
