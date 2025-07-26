namespace StudentInfoSys.Data.Entities
{
    public class EnrollmentEntity : BaseEntity
    {
        public int AbsanceCount { get; set; } = 0;

        // Foreign key
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Navigation properties
        public required StudentEntity Student { get; set; }
        public required CourseEntity Course { get; set; }
        public ICollection<ExamEntity> Exams { get; set; } = new List<ExamEntity>();
    }
}