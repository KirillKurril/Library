﻿using Microsoft.AspNetCore.Http;

namespace Library.Presentation.Services.BookImage
{
    public interface IBookImageService
    {
        public string SaveImage(IFormFile image);
    }
}