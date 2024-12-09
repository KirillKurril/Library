using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Library.Presentation.Services;

public interface IKeycloakService
{
    Task<string?> GetUserEmailById(string userId);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
}

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;

    public class KeycloakSettings
    {
        public string Authority { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

    public KeycloakService(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string?> GetUserEmailById(string userId)
    {
        var response = await _httpClient.GetAsync($"{_settings.Authority}/admin/users/{userId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("email").GetString();
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "client_id", _settings.ClientId },
            { "client_secret", _settings.ClientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

        var response = await _httpClient.PostAsync($"{_settings.Authority}/protocol/openid-connect/token", 
            new FormUrlEncodedContent(parameters));

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to refresh token");

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        return (
            root.GetProperty("access_token").GetString()!,
            root.GetProperty("refresh_token").GetString()!
        );
    }
}
