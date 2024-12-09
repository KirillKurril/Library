using Library.Application.Common.Interfaces;
using Library.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Library.Presentation.Services.AuthService
{
    public class KeycloakTokenAccessor : ITokenAccessor
    {
        private readonly KeycloakSettings _keycloakSettings;
        private readonly HttpContext? _httpContext;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache; // Для хранения refresh token

        public KeycloakTokenAccessor(
            IOptions<KeycloakSettings> options,
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient,
            IDistributedCache cache)
        {
            _keycloakSettings = options.Value;
            _httpContext = httpContextAccessor.HttpContext;
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_httpContext?.User.Identity?.IsAuthenticated == true)
            {
                var accessToken = await _httpContext.GetTokenAsync("access_token");
                if (!IsTokenExpired(accessToken))
                    return accessToken;

                var refreshToken = await _httpContext.GetTokenAsync("refresh_token");
                if (refreshToken != null)
                {
                    return await RefreshTokenAsync(refreshToken);
                }
            }

            // Если нет пользовательского контекста или обновление не удалось,
            // получаем client credentials токен
            return await GetClientCredentialsTokenAsync();
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var requestUri = $"{_keycloakSettings.Host}/realms/{_keycloakSettings.Realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", _keycloakSettings.ClientId),
            new KeyValuePair<string, string>("client_secret", _keycloakSettings.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        });

            var response = await _httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                // Если не удалось обновить, возвращаемся к client credentials
                return await GetClientCredentialsTokenAsync();
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

            // Сохраняем новый refresh token
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                await _cache.SetStringAsync(
                    $"refresh_token_{_httpContext?.User.FindFirst("sub")?.Value}",
                    tokenResponse.RefreshToken,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    });
            }

            return tokenResponse.AccessToken;
        }

        private async Task<string> GetClientCredentialsTokenAsync()
        {
            var requestUri = $"{_keycloakSettings.Host}/realms/{_keycloakSettings.Realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", _keycloakSettings.ClientId),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_secret", _keycloakSettings.ClientSecret)
        });

            var response = await _httpClient.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get token: {response.StatusCode}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse.AccessToken;
        }

        public async Task SetAuthorizationHeaderAsync(HttpClient httpClient)
        {
            var token = await GetAccessTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(1); 
        }
    }
}
