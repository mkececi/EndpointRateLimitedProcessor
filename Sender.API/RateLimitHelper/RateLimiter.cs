namespace Sender.API.RateLimitHelper
{
    /// <summary>
    /// Belirli bir endpoint için rate limiting mekanizması
    /// </summary>
    public class RateLimiter
    {
        /// <summary>
        /// Dakika başına izin verilen maksimum istek sayısı
        /// </summary>
        private readonly int _maxRequests;

        /// <summary>
        /// Rate limit uygulanacak endpoint
        /// </summary>
        private readonly string _endpoint;

        /// <summary>
        /// RateLimiter sınıfının yeni bir örneğini oluşturur
        /// </summary>
        /// <param name="endpoint">Rate limit uygulanacak endpoint</param>
        /// <param name="maxRequestsPerMinute">Dakika başına izin verilen maksimum istek sayısı</param>
        public RateLimiter(string endpoint, int maxRequestsPerMinute)
        {
            _endpoint = endpoint;
            _maxRequests = maxRequestsPerMinute;
        }

        /// <summary>
        /// Rate limit kontrolü için bekleme metodu
        /// </summary>
        public async Task WaitAsync()
        {
            await GlobalEndpointRateLimiter.WaitAsync(_endpoint, _maxRequests);
        }
    }
}
