using Library.Application.Common.Models;
using Library.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Library.Infrastructure.Services
{
    public class LocalBookImageService : IBookImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILibrarySettings _librarySettings;
        private readonly ILogger<LocalBookImageService> _logger;
        public LocalBookImageService(
            IWebHostEnvironment webHostEnvironment,
            ILibrarySettings librarySettings,
            ILogger<LocalBookImageService> logger)
        {
            _env = webHostEnvironment;
            _librarySettings = librarySettings;
            _logger = logger;   
        }
        public async Task<ResponseData<string>> SaveCoverImage(IFormFile image, HostString host, string scheme)
        {
            var isImage = await IsImageFile(image);
            if (isImage)
            {
                var extension = Path.GetExtension(image.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_env.WebRootPath, "images", "covers", fileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseData<string>(false, $"Error saving book cover: {ex.Message}");
                }

                var uriBuilder = new UriBuilder
                {
                    Scheme = scheme,
                    Host = host.Host,
                    Port = host.Port ?? -1,
                    Path = $"/images/covers/{fileName}"
                };
                var url = uriBuilder.ToString();

                return new ResponseData<string>(url);
            }
            return new ResponseData<string>(false, $"Transfered file {image.FileName} is not an image");
        }
        public ResponseData<string> GetDefaultCoverURL(HostString host, string scheme)
        {
            string defaulCoverFileName = _librarySettings.DefaulCoverFileName;

            if(defaulCoverFileName == null)
            {
                _logger.LogError("Error receiving default book cover file name from configuration");
                return new ResponseData<string>(false, "Error receiving default book cover file name from configuration");
            }

            var uriBuilder = new UriBuilder
            {
                Scheme = scheme,
                Host = host.Host,
                Port = host.Port ?? -1,
                Path = $"/images/covers/{defaulCoverFileName}"
            };
            var url = uriBuilder.ToString();

            return new ResponseData<string>(url);
        }
        private async Task<bool> IsImageFile(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
             
                if (fileBytes.Length > 8 &&
                fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47 &&
                fileBytes[4] == 0x0D && fileBytes[5] == 0x0A && fileBytes[6] == 0x1A && fileBytes[7] == 0x0A)
                 {
                     return true;
                 }

                 if (fileBytes.Length > 2 &&
                     fileBytes[0] == 0xFF && fileBytes[1] == 0xD8)
                 {
                     return true;
                 }

                 return false; 
            }
        }
    
    }
}
