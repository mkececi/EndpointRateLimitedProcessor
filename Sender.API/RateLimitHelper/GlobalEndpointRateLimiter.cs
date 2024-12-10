namespace Sender.API.RateLimitHelper
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Global endpoint bazlı rate limiting mekanizması
    /// </summary>
    public class GlobalEndpointRateLimiter
    {
        /// <summary>
        /// Endpoint bazlı semaphore havuzu
        /// </summary>
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _endpointSemaphores
            = new ConcurrentDictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Endpoint bazlı istek zaman damgası kayıtları
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _endpointRequestTimestamps
            = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        /// <summary>
        /// Garanti olması için bir dakikaya maxRequestsPerMinute almak yerine -1 olarak alıyoruz.
        /// </summary>
        private static readonly int _buffer = 1;

        /// <summary>
        /// Belirli bir endpoint için rate limit kontrolü yapar
        /// </summary>
        /// <param name="endpoint">Rate limit uygulanacak endpoint</param>
        /// <param name="maxRequestsPerMinute">Dakika başına izin verilen maksimum istek sayısı</param>
        /// <returns>Asenkron rate limit bekleme görevi</returns>
        public static async Task WaitAsync(string endpoint, int maxRequestsPerMinute)
        {
            // Endpoint için semaphore oluştur veya mevcut olanı al
            var semaphore = _endpointSemaphores.GetOrAdd(endpoint,
                _ => new SemaphoreSlim(maxRequestsPerMinute - _buffer, maxRequestsPerMinute));

            // Endpoint için zaman damgası koleksiyonunu oluştur veya mevcut olanı al
            var timestamps = _endpointRequestTimestamps.GetOrAdd(endpoint,
                _ => new ConcurrentQueue<DateTime>());

            // Semaphore izni al
            await semaphore.WaitAsync();

            try
            {
                var now = DateTime.UtcNow;

                // Bir dakikadan eski zaman damgalarını temizle
                while (timestamps.TryPeek(out DateTime oldestTimestamp) &&
                       now - oldestTimestamp > TimeSpan.FromMinutes(1))
                {
                    timestamps.TryDequeue(out _);
                    semaphore.Release();
                }

                // Yeni zaman damgasını ekle
                timestamps.Enqueue(now);
            }
            finally
            {
                // Asenkron olarak 1 dakika sonra semaphore'ları temizle
                _ = ReleaseSemaphoreAfterDelayAsync(endpoint, semaphore, maxRequestsPerMinute);
            }
        }

        /// <summary>
        /// Belirli bir süre sonra semaphore'ları temizleyen asenkron metod
        /// </summary>
        private static async Task ReleaseSemaphoreAfterDelayAsync(
            string endpoint,
            SemaphoreSlim semaphore,
            int maxRequestsPerMinute)
        {
            // 1 dakika bekle
            await Task.Delay(TimeSpan.FromMinutes(1));

            // Endpoint için zaman damgası koleksiyonunu al
            var timestamps = _endpointRequestTimestamps.GetOrAdd(endpoint,
                _ => new ConcurrentQueue<DateTime>());

            // Bir dakikadan eski zaman damgalarını temizle
            var now = DateTime.UtcNow;
            while (timestamps.TryPeek(out DateTime oldestTimestamp) &&
                   now - oldestTimestamp > TimeSpan.FromMinutes(1))
            {
                timestamps.TryDequeue(out _);
                semaphore.Release();
            }
        }
    }
}
