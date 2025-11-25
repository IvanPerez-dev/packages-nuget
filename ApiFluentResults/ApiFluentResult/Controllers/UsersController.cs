using ApiFluentResult.Services;
using ApiFluentResults.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ApiFluentResult.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : FluentResultController
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.Get(id);
            return MapResult(result).Ok().Build();
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAll();
            return MapResult(result).Ok().Build();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            var result = await _userService.Create(user);
            return MapResult(result).Created().ConflictFor<UserEmailAlreadyExistsError>().Build();
        }
    }
}
