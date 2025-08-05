using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Student;
using StudentInfoSys.Business.Operations.Student.Dtos;
using StudentInfoSys.WebApi.Models.Student;

namespace StudentInfoSys.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var result = await _studentService.GetByIdAsync(id);
            if (result.IsSucceed)
                return Ok(result.Data);
            else
                return NotFound(result.Message);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStudent(int id, StudentUpdateRequest request)
        {
            var updateStudentDto = new StudentUpdateDto
            {
                Id = id,
                StudentNumber = request.StudentNumber,
                Class = request.Class,
            };

            if (id != updateStudentDto.Id)
                return BadRequest("Id mismatch.");

            var result = await _studentService.UpdateByIdAsync(updateStudentDto);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }
    }
}
