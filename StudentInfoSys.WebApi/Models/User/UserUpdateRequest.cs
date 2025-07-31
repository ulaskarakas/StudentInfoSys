using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.User
{
    public class UserUpdateRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
