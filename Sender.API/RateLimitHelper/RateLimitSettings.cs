namespace Sender.API.RateLimitHelper
{
    /// <summary>
    /// Uygulama genelinde endpoint bazlı rate limit ayarlarını yönetir
    /// </summary>
    public class RateLimitSettings
    {
        /// <summary>
        /// Tanımlanmış endpoint rate limit konfigürasyonları
        /// </summary>
        public List<RateLimitConfiguration> RateLimits { get; set; } = new List<RateLimitConfiguration>
        {
            new RateLimitConfiguration { Endpoint = "/products/add", MaxRequestsPerMinute = 10 },
        };
    }
}
