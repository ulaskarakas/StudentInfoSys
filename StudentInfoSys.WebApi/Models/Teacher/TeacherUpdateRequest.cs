using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Teacher
{
    public class TeacherUpdateRequest
    {
        [Required]
        public required string Department { get; set; }
    }
}