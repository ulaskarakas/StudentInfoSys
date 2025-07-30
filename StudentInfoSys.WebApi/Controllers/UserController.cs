using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.User;

namespace StudentInfoSys.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }
    }
}
