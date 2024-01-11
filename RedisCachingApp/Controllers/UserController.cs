using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisCachingApp.Models;
using RedisCachingApp.Services.Interfaces;

namespace RedisCachingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

    }
}
