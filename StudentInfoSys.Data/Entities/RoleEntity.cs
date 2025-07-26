namespace StudentInfoSys.Data.Entities
{
    public class RoleEntity : BaseEntity
    {   
        public required string Name { get; set; }

        // Navigation properties
        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}