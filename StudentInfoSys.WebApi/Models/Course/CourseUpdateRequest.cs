using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Course
{
    public class CourseUpdateRequest
    {
        [Required]
        public required string CourseCode { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required int TeacherId { get; set; }
    }
}