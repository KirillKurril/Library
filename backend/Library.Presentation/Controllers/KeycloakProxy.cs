using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.Application.Common.Interfaces;
using Library.Application.Common.Settings;

namespace Library.Presentation.Controllers
{
    [ApiController]
    [Route("api/keycloak")]
    public class KeycloakProxyController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakSettings _keycloakSettings;
        private readonly ITokenAccessor _tokenAccessor;

        public KeycloakProxyController(
            HttpClient httpClient,
            ITokenAccessor tokenAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _tokenAccessor = tokenAccessor;
            _keycloakSettings = configuration
                .GetSection("Keycloak")
                .Get<KeycloakSettings>();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ProxyGet()
        {
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            string requestUrl = $"{_keycloakSettings.Host}/{Request.Path}{Request.QueryString}";
            requestUrl = requestUrl.Replace("api/keycloak", "");

            var response = await _httpClient.GetAsync(requestUrl);

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
