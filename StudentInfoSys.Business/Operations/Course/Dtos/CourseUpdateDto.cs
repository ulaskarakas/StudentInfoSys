using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Course.Dtos
{
    public class CourseUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string CourseCode { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required int TeacherId { get; set; }
    }
}
