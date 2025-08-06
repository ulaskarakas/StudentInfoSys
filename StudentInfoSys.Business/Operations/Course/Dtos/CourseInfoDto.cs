using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Course.Dtos
{
    public class CourseInfoDto
    {
        public int Id { get; set; }
        [Required]
        public required string CourseCode { get; set; }
        [Required]
        public required string Title { get; set; }
        public int TeacherId { get; set; }
    }
}
