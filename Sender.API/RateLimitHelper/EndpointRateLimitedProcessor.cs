namespace Sender.API.RateLimitHelper
{
    /// <summary>
    /// Endpoint bazlı rate limiting işlemcisi
    /// </summary>
    public class EndpointRateLimitedProcessor : IEndpointRateLimitedProcessor
    {
        /// <summary>
        /// Endpoint bazlı rate limiter'ları içeren sözlük
        /// </summary>
        private readonly Dictionary<string, RateLimiter> _rateLimiters;

        /// <summary>
        /// Rate limit ayarları
        /// </summary>
        private readonly RateLimitSettings _rateLimitSettings;

        /// <summary>
        /// EndpointRateLimitedProcessor sınıfının yeni bir örneğini oluşturur
        /// </summary>
        /// <param name="rateLimitSettings">Rate limit ayarları</param>
        public EndpointRateLimitedProcessor(RateLimitSettings rateLimitSettings)
        {
            _rateLimitSettings = rateLimitSettings;
            _rateLimiters = rateLimitSettings.RateLimits.ToDictionary(
                limit => limit.Endpoint,
                limit => new RateLimiter(limit.Endpoint, limit.MaxRequestsPerMinute)
            );
        }

        /// <summary>
        /// Endpoint bazlı rate limit uygulanarak toplu işlem gerçekleştirir
        /// </summary>
        /// <typeparam name="T">İşlenecek öğe türü</typeparam>
        /// <param name="endpoint">İşlem yapılacak endpoint</param>
        /// <param name="items">İşlenecek öğeler</param>
        /// <param name="action">Her öğe için gerçekleştirilecek eylem</param>
        public async Task ProcessAsync<T>(
            string endpoint,
            IEnumerable<T> items,
            Func<T, Task> action)
        {
            // Endpoint için rate limiter var mı kontrol et
            if (!_rateLimiters.TryGetValue(endpoint, out var rateLimiter))
            {
                throw new InvalidOperationException($"Rate limit for endpoint '{endpoint}' is not configured.");
            }

            // Paralel olarak öğeleri işle
            var tasks = items.Select(async item =>
            {
                // Rate limit kontrolü
                await rateLimiter.WaitAsync();
                try
                {
                    // Öğe üzerinde eylemi gerçekleştir
                    await action(item);
                }
                finally
                {
                    // İşlem tamamlandığını log et
                    Console.WriteLine($"Processed item: {DateTime.Now}");
                }
            });

            // Tüm görevlerin tamamlanmasını bekle
            await Task.WhenAll(tasks);
        }
    }
}
