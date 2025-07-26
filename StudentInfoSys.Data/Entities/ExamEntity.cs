namespace StudentInfoSys.Data.Entities
{
    public class ExamEntity : BaseEntity
    {
        public required string Title { get; set; }
        public DateTime ExamDate { get; set; }
        public int? Grade { get; set; }

        // Foreign key
        public int EnrollmentId { get; set; }

        // Navigation properties
        public required EnrollmentEntity Enrollment { get; set; } 
    }
}