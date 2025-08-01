using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.User
{
    public class UserRegisterRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one role must be selected.")]
        public required List<string> RoleNames { get; set; }
    }
}
