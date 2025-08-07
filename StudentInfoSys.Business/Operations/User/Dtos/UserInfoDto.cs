using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.User.Dtos
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public required List<string> Roles { get; set; }
    }
}