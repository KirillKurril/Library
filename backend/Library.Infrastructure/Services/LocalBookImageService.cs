using Library.Presentation.Services.BookImage;
using Library.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Library.Presentation.Services
{
    public class LocalBookImageService : IBookImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalBookImageService> _logger;
        public LocalBookImageService(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            ILogger<LocalBookImageService> logger)
        {
            _env = webHostEnvironment;
            _configuration = configuration;
            _logger = logger;   
        }
        public async Task<ResponseData<string>> SaveImage(IFormFile image, HostString host, string scheme)
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
                    _logger.LogError($"Error saving book cover: {ex.Message}");
                    return new ResponseData<string>(false, $"Error saving book cover: {ex.Message}");
                }

                var baseUrl = $"{scheme}://{host}";
                var url = Path.Combine(baseUrl, "images", "covers", filePath);

                _logger.LogInformation($"Saved image to url {url} and to path {filePath}");

                return new ResponseData<string>(url);
            }
            return new ResponseData<string>(false, $"Transfered file {image.FileName} is not an image");
        }
        public ResponseData<string> GetDefaultCoverImage(HostString host, string scheme)
        {
            string defaulCoverFileName = "";
            defaulCoverFileName = _configuration.GetValue<string>("LibrarySettings:LibrarySettings");

            if(defaulCoverFileName == null)
            {
                _logger.LogError("Error receiving default book cover file name from configuration");
                return new ResponseData<string>(false, "Error receiving default book cover file name from configuration");
            }
            
            var baseUrl = $"{scheme}://{host}";
            var url = Path.Combine(baseUrl, "images", "covers", defaulCoverFileName);

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
