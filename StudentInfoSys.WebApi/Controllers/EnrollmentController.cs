using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Enrollment;
using StudentInfoSys.Business.Operations.Enrollment.Dtos;
using StudentInfoSys.WebApi.Models.Enrollment;

namespace StudentInfoSys.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CreateEnrollment(EnrollmentCreateDto dto)
        {
            var result = await _enrollmentService.CreateAsync(dto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetEnrollmentById(int id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateEnrollment(int id, EnrollmentUpdateRequest request)
        {
            var updateEnrollmentDto = new EnrollmentUpdateDto
            {
                Id = id,
                AbsanceCount = request.AbsanceCount
            };
            var result = await _enrollmentService.UpdateByIdAsync(updateEnrollmentDto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var result = await _enrollmentService.DeleteByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Message);
        }
    }
}