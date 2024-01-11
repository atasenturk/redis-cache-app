using StackExchange.Redis;

namespace RedisCachingApp.Services.Interfaces
{
    public interface ICacheService
    {
        Task Clear(RedisKey key);
        void ClearAll();
        Task<bool> KeyExistsAsync(RedisKey key);
        Task<T> GetOrAddAsync<T>(RedisKey key, Func<Task<T>> action) where T : class;
        Task<List<T>> GetOrAddListOfObject<T>(RedisKey key, List<T> list) where T : class;
        Task<string> GetValueAsync(RedisKey key);
        Task<bool> SetValueAsync(RedisKey key, string value);
        T GetOrAdd<T>(RedisKey key, Func<T> action) where T : class;
        Task<bool> SetHashAsync<T>(RedisKey key, T obj) where T : class;
        Task<T> GetHashAsync<T>(RedisKey key) where T : class, new();
    }
}
