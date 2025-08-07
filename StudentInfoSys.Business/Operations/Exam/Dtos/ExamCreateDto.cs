using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Exam.Dtos
{
    public class ExamCreateDto
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required DateTime ExamDate { get; set; }
        public int? Grade { get; set; }
        [Required]
        public required int EnrollmentId { get; set; }
    }
}
