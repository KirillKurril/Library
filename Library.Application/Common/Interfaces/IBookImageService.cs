using Library.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Library.Presentation.Services.BookImage
{
    public interface IBookImageService
    {
        public Task<ResponseData<string>> SaveImage(IFormFile image, HostString host, string scheme);
    }
}
