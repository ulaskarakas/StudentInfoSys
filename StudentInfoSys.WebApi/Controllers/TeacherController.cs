using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Teacher;
using StudentInfoSys.Business.Operations.Teacher.Dtos;
using StudentInfoSys.WebApi.Models.Teacher;

namespace StudentInfoSys.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            var result = await _teacherService.GetByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTeacher(int id, TeacherUpdateRequest request)
        {
            var updateTeacherDto = new TeacherUpdateDto
            {
                Id = id,
                Department = request.Department
            };

            if (id != updateTeacherDto.Id)
                return BadRequest("Id mismatch.");

            var result = await _teacherService.UpdateByIdAsync(updateTeacherDto);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
    }
}
