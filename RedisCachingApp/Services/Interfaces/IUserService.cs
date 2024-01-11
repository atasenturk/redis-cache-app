using RedisCachingApp.Models;

namespace RedisCachingApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersFromCacheAsync();
    }
}
