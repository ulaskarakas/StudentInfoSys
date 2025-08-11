using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Course;
using StudentInfoSys.Business.Operations.Course.Dtos;
using StudentInfoSys.WebApi.Filters;
using StudentInfoSys.WebApi.Models.Course;

namespace StudentInfoSys.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // Create
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> CreateCourse(CourseCreateRequest request)
        {
            var createCourseDto = new CourseCreateDto
            {
                CourseCode = request.CourseCode,
                Title = request.Title,
                TeacherId = request.TeacherId
            };
            var result = await _courseService.CreateAsync(createCourseDto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        // Read
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var result = await _courseService.GetByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Data);
        }

        // Update
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> UpdateCourse(int id, CourseUpdateRequest request)
        {
            var updateCourseDto = new CourseUpdateDto
            {
                Id = id,
                CourseCode = request.CourseCode,
                Title = request.Title,
                TeacherId = request.TeacherId
            };

            if (id != updateCourseDto.Id)
                return BadRequest("Id mismatch.");

            var result = await _courseService.UpdateByIdAsync(updateCourseDto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        // Delete
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Message);
        }
    }
}