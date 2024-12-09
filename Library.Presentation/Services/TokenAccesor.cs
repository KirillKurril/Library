using Library.Application.Common.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Library.Application.Common.Settings;
using Microsoft.AspNetCore.Authentication;

namespace Library.Presentation.Services
{
    public class TokenAccesor : ITokenAccessor
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakSettings _settings;
        private readonly HttpContext _httpContext;
        public TokenAccesor(
            IHttpContextAccessor contextAccessor,
            KeycloakSettings settings,
            HttpClient httpClient)
        {
            _httpContext = contextAccessor.HttpContext;
            _settings = settings;
            _httpClient = httpClient;
        }
        public async Task<string> GetAccessTokenAsync()
        {

            if (_httpContext.User.Identity.IsAuthenticated)
            {
                return await _httpContext.GetTokenAsync("access_token");
            }

            var requestUri =
                $"{_settings.Host}/realms/{_settings.Realm}/protocol/openid-connect/token";

            HttpContent content = new FormUrlEncodedContent([
                new KeyValuePair<string,string>("client_id",_settings.ClientId),
                new KeyValuePair<string,string>("grant_type","client_credentials"),
                new KeyValuePair<string,string>("client_secret",_settings.ClientSecret)
            ]);

            var response = await _httpClient.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.StatusCode.ToString());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(jsonString)["access_token"].GetValue<string>();
        }
        public async Task SetAuthorizationHeaderAsync(HttpClient httpClient)
        {
            string token = await GetAccessTokenAsync();
            httpClient
            .DefaultRequestHeaders
            .Authorization = new AuthenticationHeaderValue("bearer", token); ;
        }
        public Task<string> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
