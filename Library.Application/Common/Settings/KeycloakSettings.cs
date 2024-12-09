namespace Library.Application.Common.Settings
{
    public class KeycloakSettings
    {
        public string Host { get; } = null!;
        public string Realm { get; } = null!;
        public string ClientId { get; } = null!;
        public string ClientSecret { get; } = null!;
    }
}