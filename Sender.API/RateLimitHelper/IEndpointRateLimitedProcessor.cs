namespace Sender.API.RateLimitHelper
{
    /// <summary>
    /// Endpoint bazlı rate limiting işlemcisi için arayüz
    /// </summary>
    public interface IEndpointRateLimitedProcessor
    {
        /// <summary>
        /// Endpoint bazlı rate limit uygulanarak toplu işlem gerçekleştirir
        /// </summary>
        Task ProcessAsync<T>(string endpoint, IEnumerable<T> items, Func<T, Task> action);
    }
}
