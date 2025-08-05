using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.User
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
