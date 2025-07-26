namespace StudentInfoSys.Data.Entities
{
    public class StudentEntity : BaseEntity
    {
        public required string StudentNumber { get; set; }
        public required string Class { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation properties
        public required UserEntity User { get; set; }
        public ICollection<EnrollmentEntity> Enrollments { get; set; } = new List<EnrollmentEntity>();
    }
}