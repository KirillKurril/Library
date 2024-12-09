﻿namespace Library.Application.Common.Interfaces
{
    public interface ITokenAccessor
    {
        Task<string> GetAccessTokenAsync();
        Task SetAuthorizationHeaderAsync(HttpClient httpClient);
        Task<string> RefreshTokenAsync(string refreshToken);
    }
}