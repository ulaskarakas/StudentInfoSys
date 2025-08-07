using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Enrollment
{
    public class EnrollmentCreateRequest
    {
        [Required]
        public required int StudentId { get; set; }
        [Required]
        public required int CourseId { get; set; }
        [Required]
        public required int AbsanceCount { get; set; } = 0;
    }
}