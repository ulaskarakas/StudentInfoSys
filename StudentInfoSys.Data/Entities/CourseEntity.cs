namespace StudentInfoSys.Data.Entities
{
    public class CourseEntity : BaseEntity
    {
        public required string CourseCode { get; set; }
        public required string Title { get; set; }

        // Foreign key
        public int TeacherId { get; set; }

        // Navigation properties
        public required TeacherEntity Teacher { get; set; }
        public ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
    }
}