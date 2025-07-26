namespace StudentInfoSys.Data.Entities
{
    public class TeacherEntity : BaseEntity
    {
        public required string Department { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public required UserEntity User { get; set; }
        public ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();
    }
}