using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.User;
using StudentInfoSys.WebApi.Filters;
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
        [TimeControlFilter("23:59", "07:00")]
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
                RoleNames = request.RoleNames
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
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }
    }
}