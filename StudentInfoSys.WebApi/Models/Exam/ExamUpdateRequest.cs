using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Exam
{
    public class ExamUpdateRequest
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required DateTime ExamDate { get; set; }
        [Required]
        public int? Grade { get; set; }
    }
}