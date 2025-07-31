namespace StudentInfoSys.Business.Operations.User.Dtos
{
    public class UserRoleAssignRequestDto
    {
        public required string Email { get; set; }
        public required string RoleName { get; set; }
    }
}
