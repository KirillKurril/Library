using Library.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Library.Application.Common.Interfaces
{
    public interface IBookImageService
    {
        public Task<ResponseData<string>> SaveCoverImage(IFormFile image, HostString host, string scheme);
        public ResponseData<string> GetDefaultCoverURL(HostString host, string scheme);
    }
}
