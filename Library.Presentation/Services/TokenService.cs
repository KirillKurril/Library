using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Library.Presentation.Services;

public interface ITokenService
{
    Task SaveRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiration);
    Task<string?> GetRefreshTokenAsync(string userId);
    Task RemoveRefreshTokenAsync(string userId);
}

public class TokenService : ITokenService
{
    private readonly IDistributedCache _cache;
    private const string KeyPrefix = "refresh_token:";

    public TokenService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SaveRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _cache.SetStringAsync($"{KeyPrefix}{userId}", refreshToken, options);
    }

    public async Task<string?> GetRefreshTokenAsync(string userId)
    {
        return await _cache.GetStringAsync($"{KeyPrefix}{userId}");
    }

    public async Task RemoveRefreshTokenAsync(string userId)
    {
        await _cache.RemoveAsync($"{KeyPrefix}{userId}");
    }
}
