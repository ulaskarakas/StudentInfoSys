using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Enrollment.Dtos
{
    public class EnrollmentUpdateDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required int AbsanceCount { get; set; }
    }
}