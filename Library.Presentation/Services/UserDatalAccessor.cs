using System.Text.Json;
using System.Text.Json.Nodes;
using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Library.Presentation.Services;

public class UserDatalAccessor : IUserDataAccessor
{
    private readonly HttpClient _httpClient;
    private readonly ITokenAccessor _tokenAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserDatalAccessor> _logger;
    public UserDatalAccessor(
        ITokenAccessor tokenAccessor,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<UserDatalAccessor> logger)
    {
        _httpClient = httpClient;
        _tokenAccessor = tokenAccessor;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseData<JsonElement>> GetUserDataAsJson(Guid userId)
    {
        await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);

        var response = await _httpClient.GetAsync($"users/{userId}");
        if (!response.IsSuccessStatusCode)
            return new ResponseData<JsonElement>(false, $"Unable to fetch user {userId} data");

        var content = await response.Content.ReadAsStringAsync();
        var userData = JsonSerializer.Deserialize<JsonElement>(content);

        return new ResponseData<JsonElement>(userData);
    }

    public async Task<bool> UserExist(Guid userId)
    {
        var userDataResponse = await GetUserDataAsJson(userId);
        return userDataResponse.Success
                && !userDataResponse.Data.TryGetProperty("error", out var notFoundString);
    }
    public async Task<ResponseData<Dictionary<Guid, string>>> GetUsersEmailsByIds(IEnumerable<Guid> userIds)
    {
        await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);

        var countResponse = await _httpClient.GetAsync($"users/count");

        if(!countResponse.IsSuccessStatusCode)
        {
            return new ResponseData<Dictionary<Guid, string>>(false, "Unable to fetch users count from keycloak");
        }

        var content = await countResponse.Content.ReadAsStringAsync();
        var userCount = JsonSerializer.Deserialize<int>(content);

        var step = _configuration.GetValue<int>("UserSFetchingStep");

        var result = new Dictionary<Guid, string>();
        var tasks = new List<Task>();
        try
        {
            for (int i = 0; i < userCount; i += step)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var usersInfoResponse = await _httpClient.GetAsync($"users?first={i}&max={step}");
                    if (!usersInfoResponse.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Unable to fetch users info id({i} - {i + step}) from keycloak");
                        return;
                    }
                    var usersData = await usersInfoResponse.Content.ReadAsStringAsync();
                    var usersArray = JsonSerializer.Deserialize<JsonElement[]>(usersData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    foreach (var userElement in usersArray)
                    {
                        if (userElement.TryGetProperty("id", out var idProperty) &&
                            userElement.TryGetProperty("email", out var emailProperty))
                        {
                            var userId = idProperty.GetGuid();
                            var email = emailProperty.GetString();

                            if (userIds.Contains(userId))
                            {
                                result[userId] = email;
                            }
                        }
                    }
                }));
            }
        }
        catch(Exception ex)
        {
            return new ResponseData<Dictionary<Guid, string>>(false,
                $"Could not parse user data for element: {ex.Message}");
        }

        await Task.WhenAll(tasks);

        return new ResponseData<Dictionary<Guid, string>>(result);
    }

   
}
