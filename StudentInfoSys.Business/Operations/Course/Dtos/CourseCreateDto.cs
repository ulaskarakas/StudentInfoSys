using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Course.Dtos
{
    public class CourseCreateDto
    {
        [Required]
        public required string CourseCode { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required int TeacherId { get; set; }
    }
}