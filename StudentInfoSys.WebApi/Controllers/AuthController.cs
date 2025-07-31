using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.User;
using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.WebApi.Jwt;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");

            var userLoginDto = new UserLoginDto
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _userService.LoginAsync(userLoginDto);

            if (!result.IsSucceed || result.Data == null)
            {
                return BadRequest(result.Message);
            }

            var user = result.Data;

            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Roles = user.Roles,
                SecretKey = configuration["Jwt:SecretKey"]!,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
            });

            return Ok(new UserLoginResponse
            {
                Message = "Login completed successfully",
                Token = token
            });
        }

        [HttpGet("me")]
        [Authorize] // If there is no token, there is no response
        public IActionResult GetMyUser()
        {
            return Ok();
        }
    }
}
