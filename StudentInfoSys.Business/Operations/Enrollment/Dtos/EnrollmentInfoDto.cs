using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Enrollment.Dtos
{
    public class EnrollmentInfoDto
    {
        public int Id { get; set; }
        [Required]
        public required int StudentId { get; set; }
        [Required]
        public required int CourseId { get; set; }
        [Required]
        public required int AbsanceCount { get; set; }
    }
}