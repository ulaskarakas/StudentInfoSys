using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Role
{
    public class RoleCreateRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
