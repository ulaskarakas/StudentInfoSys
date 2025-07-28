using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Role;
using StudentInfoSys.Business.Operations.Role.Dtos;
using StudentInfoSys.WebApi.Models.Role;

namespace StudentInfoSys.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService adminService)
        {
            _roleService = adminService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(RoleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");

            var RoleCreateDto = new RoleCreateDto { Name = request.Name };

            var result = await _roleService.CreateAsync(RoleCreateDto);

            if (result.IsSucceed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

    }
}
