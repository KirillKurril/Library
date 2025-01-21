using Library.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Library.Presentation.Services.BookImage
{
    public interface IBookImageService
    {
        public Task<ResponseData<string>> SaveCoverImage(IFormFile image, HostString host, string scheme);
        public ResponseData<string> GetDefaultCoverURL(HostString host, string scheme);
    }
}
