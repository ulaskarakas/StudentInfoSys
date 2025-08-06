using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Course.Dtos
{
    public class CourseCreateDto
    {
        [Required]
        public required string CourseCode { get; set; }
        [Required]
        public required string Title { get; set; }
        public int TeacherId { get; set; }
    }
}
