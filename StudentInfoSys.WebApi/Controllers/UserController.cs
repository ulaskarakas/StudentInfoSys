using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.User;
using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.WebApi.Models.User;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateRequest request)
        {
            var updateUserDto = new UserUpdateDto
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                Email = request.Email,
                BirthDate = request.BirthDate,
            };

            if (id != updateUserDto.Id)
                return BadRequest("Id mismatch.");

            var result = await _userService.UpdateByIdAsync(updateUserDto);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }

        [HttpPost("role/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(UserRoleAssignRequestDto request)
        {
            var result = await _userService.AssignRoleAsync(request.Email, request.RoleName);
            if (result.IsSucceed)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpPost("role/remove")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole(UserRoleRemoveRequestDto request)
        {
            var result = await _userService.RemoveRoleAsync(request.Email, request.RoleName);
            if (result.IsSucceed)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }
    }
}
