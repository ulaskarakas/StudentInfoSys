using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentInfoSys.Business.Operations.Exam;
using StudentInfoSys.Business.Operations.Exam.Dtos;
using StudentInfoSys.WebApi.Filters;
using StudentInfoSys.WebApi.Models.Exam;

namespace StudentInfoSys.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> CreateExam(ExamCreateRequest request)
        {
            var updateExamDto = new ExamCreateDto
            {
                Title = request.Title,
                ExamDate = request.ExamDate,
                Grade = request.Grade,
                EnrollmentId = request.EnrollmentId
            };
            var result = await _examService.CreateAsync(updateExamDto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> GetExamById(int id)
        {
            var result = await _examService.GetByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> UpdateExam(int id, ExamUpdateRequest request)
        {
            var updateExamDto = new ExamUpdateDto
            {
                Id = id,
                Title = request.Title,
                ExamDate = request.ExamDate,
                Grade = request.Grade
            };
            var result = await _examService.UpdateByIdAsync(updateExamDto);
            if (!result.IsSucceed) 
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        [TimeControlFilter("23:59", "07:00")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteByIdAsync(id);
            if (!result.IsSucceed) 
                return NotFound(result.Message);
            return Ok(result.Message);
        }
    }
}
