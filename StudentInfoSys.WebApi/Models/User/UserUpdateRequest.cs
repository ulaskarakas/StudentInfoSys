namespace StudentInfoSys.WebApi.Models.User
{
    public class UserUpdateRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
