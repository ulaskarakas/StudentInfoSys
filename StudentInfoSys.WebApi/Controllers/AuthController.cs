using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.User;
using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.WebApi.Models.User;

namespace StudentInfoSys.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");

            var UserRegisterDto = new UserRegisterDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate
            };

            var result = await _userService.RegisterAsync(UserRegisterDto);

            if (result.IsSucceed)
                return Ok();
            else
                return BadRequest(result.Message);
        }
    }
}
