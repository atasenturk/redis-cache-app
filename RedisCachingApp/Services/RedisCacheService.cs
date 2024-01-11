using StackExchange.Redis;
using Newtonsoft.Json;
using RedisCachingApp.Extensions;
using RedisCachingApp.Models;
using RedisCachingApp.Services.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RedisCachingApp.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisCon;
        private readonly IDatabase _cache;
        public static TimeSpan ExpireTime => TimeSpan.FromDays(1);

        public RedisCacheService(IConnectionMultiplexer redisCon)
        {
            _redisCon = redisCon;
            _cache = redisCon.GetDatabase();
        }

        public async Task Clear(RedisKey key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public void ClearAll()
        {
            var endpoints = _redisCon.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _redisCon.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }
        public async Task<bool> KeyExistsAsync(RedisKey key)
        {
            return await _cache.KeyExistsAsync(key);
        }

        public async Task<T> GetOrAddAsync<T>(RedisKey key, Func<Task<T>> action) where T : class
        {
            var result = await _cache.StringGetAsync(key);
            if (result.IsNull)
            {
                result = JsonSerializer.Serialize(await action());
                await SetValueAsync(key, result);
            }
            return JsonSerializer.Deserialize<T>(result);
        }

        public async Task<string> GetValueAsync(RedisKey key)
        {
            return await _cache.StringGetAsync(key);
        }

        public async Task<bool> SetValueAsync(RedisKey key, string value)
        {
            return await _cache.StringSetAsync(key, value, ExpireTime);
        }

        public async Task<List<T>> GetOrAddListOfObject<T>(RedisKey key, List<T> list) where T : class
        {
            if (!await KeyExistsAsync(key))
            {
                var jsonValue = JsonConvert.SerializeObject(list);
                _cache.StringSet(key, jsonValue);
            }

            var value = await _cache.StringGetAsync(key);
            return JsonConvert.DeserializeObject<List<T>>(value);
        }

        public T GetOrAdd<T>(RedisKey key, Func<T> action) where T : class
        {
            var result = _cache.StringGet(key);
            if (result.IsNull)
            {
                result = JsonSerializer.SerializeToUtf8Bytes(action());
                _cache.StringSet(key, result, ExpireTime);
            }
            return JsonSerializer.Deserialize<T>(result);
        }

        public async Task<bool> SetHashAsync<T>(RedisKey key, T obj) where T : class
        {
            var hashEntries = obj.ToHashEntries();

            await _cache.HashSetAsync(key, hashEntries);
            return await _cache.KeyExpireAsync(key, ExpireTime);
        }

        public async Task<T> GetHashAsync<T>(RedisKey key) where T : class, new()

        {
            var hashEntries = await _cache.HashGetAllAsync(key);

            if (hashEntries.Length == 0)
                return null;

            var obj = hashEntries.ConvertFromRedis<T>();
            return obj;
        }

    }
}
