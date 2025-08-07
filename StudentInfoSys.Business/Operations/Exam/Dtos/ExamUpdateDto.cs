using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Exam.Dtos
{
   public class ExamUpdateDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required DateTime ExamDate { get; set; }
        [Required]
        public int? Grade { get; set; }
    }
}
