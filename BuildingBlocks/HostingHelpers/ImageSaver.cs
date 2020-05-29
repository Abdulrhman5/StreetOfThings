using CommonLibrary;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostingHelpers
{
    public class ImageSaver : IImageSaver
    {
        string[] ImageMimeType = new string[]
        {
            "image/jpg",
            "image/jpeg",
            "image/pjpeg",
            "image/gif",
            "image/x-png",
            "image/png",
            "image/svg+xml",
            "image/svg",
        };

        string[] ImageExtension = new string[]
        {
            ".jpg",
            ".png",
            ".gif",
            ".jpeg",
            ".svg",
        };

        private string contentRoot;

        private ILogger<ImageSaver> _logger;
        public ImageSaver(IConfiguration configs, ILogger<ImageSaver> logger)
        {
            contentRoot = configs[WebHostDefaults.ContentRootKey];
            _logger = logger;
        }

        public async Task<CommandResult<SavedImageViewModel>> SaveImage(IFormFile image)
        {
            if (image is null)
            {
                return new CommandResult<SavedImageViewModel>(
                    new ErrorMessage
                    {
                        ErrorCode = "IMAGE.NULL",
                        Message = "Please send a valid data",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
            }

            if (!ImageMimeType.Any(m => m.EqualsIC(image.ContentType))
                || !ImageExtension.Any(e => e.EqualsIC(Path.GetExtension(image.FileName))))
            {
                return new CommandResult<SavedImageViewModel>(new ErrorMessage
                {
                    ErrorCode = "IMAGE.NOT.IMAGE",
                    Message = "Please send image",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            if (image.Length == 0)
            {
                return new CommandResult<SavedImageViewModel>(
                    new ErrorMessage
                    {
                        ErrorCode = "IMAGE.TOO.SMALL",
                        Message = "Please send a valid image",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
            }


            if (image.Length > 4 * 1024 * 1024)
            {
                return new CommandResult<SavedImageViewModel>(
                    new ErrorMessage
                    {
                        ErrorCode = "IMAGE.TOO.LARGE",
                        Message = "Please send a smaller image",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
            }

            var name = Guid.NewGuid();
            var withExtension = name + Path.GetExtension(image.FileName);
            var path = Path.Combine(contentRoot, "Assets", "Images", "Profile");
            Directory.CreateDirectory(path);
            var full = Path.Combine(path, withExtension);

            try
            {
                using (Stream s = new FileStream(full, FileMode.OpenOrCreate))
                {
                    await image.CopyToAsync(s);
                }
                
                return new CommandResult<SavedImageViewModel>(new SavedImageViewModel
                {
                    Name = name,
                    Path = full,                     
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while trying to save an image to the desk");
                return new CommandResult<SavedImageViewModel>(new ErrorMessage
                {
                    ErrorCode = "IMAGE.ERROR",
                    Message = "An error occurred while trying to save the image",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                });
            }
        }
    }
}