using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.User
{
    public class UserLoginRequest
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
