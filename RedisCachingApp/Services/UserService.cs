using RedisCachingApp.Models;
using RedisCachingApp.Services.Interfaces;

namespace RedisCachingApp.Services
{
    public class UserService : IUserService
    {
        public ICacheService _cacheService { get; set; }
        public static List<User> Users => new()
        {
            new User{Id = 1, Name = "Ata",Age = 23},
            new User{Id = 2, Name = "Sule", Age = 25}
        };
        public UserService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await GetUsersFromCacheAsync();
        }

        public async Task<List<User>> GetUsersFromCacheAsync()
        {
            var key = "getallusers";

            var users = await _cacheService.GetOrAddListOfObject(key, Users);

            return users;
        }

    }
}
