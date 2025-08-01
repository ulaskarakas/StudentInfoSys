using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.User
{
    public class UserLoginResponse
    {
        [Required]
        public required string Message { get; set; }
        [Required]
        public required string Token { get; set; }
    }
}
