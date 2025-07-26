namespace StudentInfoSys.Data.Entities
{
    public class UserEntity : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime BirthDate { get; set; }

        // Navigation properties
        public StudentEntity? Student { get; set; }
        public TeacherEntity? Teacher { get; set; }
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}