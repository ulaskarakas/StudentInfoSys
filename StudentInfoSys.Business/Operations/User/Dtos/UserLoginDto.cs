using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.User.Dtos
{
    public class UserLoginDto
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
} 