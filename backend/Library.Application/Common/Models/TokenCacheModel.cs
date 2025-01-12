namespace Library.Application.Common.Models
{
    public class TokenCacheModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
