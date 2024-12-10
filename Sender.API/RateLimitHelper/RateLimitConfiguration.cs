namespace Sender.API.RateLimitHelper
{
    /// <summary>
    /// Endpoint için rate limit yapılandırma ayarlarını temsil eder
    /// </summary>
    public class RateLimitConfiguration
    {
        /// <summary>
        /// Rate limit uygulanacak endpoint URL'si
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Dakika başına izin verilen maksimum istek sayısı
        /// </summary>
        public int MaxRequestsPerMinute { get; set; }
    }
}
