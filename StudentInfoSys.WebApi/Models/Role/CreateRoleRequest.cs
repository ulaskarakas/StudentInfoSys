using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Role
{
    public class RoleCreateRequest
    {
        [Required]
        public required string Name { get; set; }
    }
}
