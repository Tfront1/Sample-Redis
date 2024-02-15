namespace Sample_Redis.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
            where T : class;

        //Factory pattern for caching, realy comfortable thing in my opinion:
        Task<T?> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
            where T : class;

        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
            where T : class;

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
    }
}
