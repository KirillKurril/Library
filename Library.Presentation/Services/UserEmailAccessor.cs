using System.Text.Json;
using Library.Application.Common.Interfaces;

namespace Library.Presentation.Services;

public class UserEmailAccessor : IUserEmailAccessor
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAccessor _tokenAccessor;
    public UserEmailAccessor(
        ITokenAccessor tokenAccessor,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _tokenAccessor = tokenAccessor;
    }

       public async Task<string?> GetUserEmailById(string userId)
    {
        await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);

        var response = await _httpClient.GetAsync($"users/{userId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<JsonElement>(content);
        return user.GetProperty("email").GetString();
    }

    public async Task<Dictionary<string, string>> GetUsersEmailsByIds(IEnumerable<string> userIds)
    {
        var result = new Dictionary<string, string>();

        foreach (var userId in userIds)
        {
            var email = await GetUserEmailById(userId);
            if (email != null)
            {
                result[userId] = email;
            }
        }

        return result;
    }

   
}
