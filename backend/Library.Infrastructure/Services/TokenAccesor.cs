using Library.Application.Common.Interfaces;
using System.Net.Http.Headers;
using Library.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Library.Application.Common.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Library.Presentation.Services
{
    public class TokenAccesor : ITokenAccessor
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakSettings _settings;
        private readonly HttpContext _httpContext;
        private readonly IDistributedCache _cache;
        private readonly string _tokenKey;
        public TokenAccesor(
            IHttpContextAccessor contextAccessor,
            KeycloakSettings settings,
            HttpClient httpClient,
            IDistributedCache cache,
            IConfiguration configuration)
        {
            _httpContext = contextAccessor.HttpContext;
            _settings = settings;
            _httpClient = httpClient;
            _cache = cache;
            _tokenKey = configuration.GetValue<string>("Redis:AccessTokenKey");
        }
        public async Task<string> GetAccessTokenAsync()
        {
            var cachedToken = await GetCachedTokenAsync();
            
            if (cachedToken != null && DateTime.UtcNow < cachedToken.ExpiresAt)
            {
                return cachedToken.AccessToken;
            }

            return await RequestNewTokenAsync();
        }

        private async Task<string> RequestNewTokenAsync()
        {
            var requestUri = "protocol/openid-connect/token";

            var content = new FormUrlEncodedContent([
                new KeyValuePair<string,string>("client_id", _settings.ClientId),
                new KeyValuePair<string,string>("grant_type", "client_credentials"),
                new KeyValuePair<string,string>("client_secret", _settings.ClientSecret)
            ]);

                var response = await _httpClient.PostAsync(requestUri, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Token request failed with status {response.StatusCode}. Error: {errorContent}");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(jsonString);

                var expiration_in = TimeSpan.FromSeconds(
                    tokenResponse.GetProperty("expires_in").GetInt32() - 60);

                var tokenModel = new TokenCacheModel
                {
                    AccessToken = tokenResponse
                        .GetProperty("access_token")
                        .GetString()!,
                    ExpiresAt = DateTime.UtcNow + expiration_in
                };

                await CacheTokenAsync(tokenModel, expiration_in);
                return tokenModel.AccessToken;
        }
        private async Task CacheTokenAsync(TokenCacheModel token, TimeSpan expiration_in)
        {
            var tokenJson = JsonSerializer.Serialize(token);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration_in
            };

            await _cache.SetStringAsync(_tokenKey, tokenJson, options);
        }

        private async Task<TokenCacheModel> GetCachedTokenAsync()
        {
            var cachedTokenJson = await _cache.GetStringAsync(_tokenKey);
            if (string.IsNullOrEmpty(cachedTokenJson))
                return null;

            return JsonSerializer.Deserialize<TokenCacheModel>(cachedTokenJson);
        }

        public async Task SetAuthorizationHeaderAsync(HttpClient httpClient)
        {
            string token = await GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
    }
}
