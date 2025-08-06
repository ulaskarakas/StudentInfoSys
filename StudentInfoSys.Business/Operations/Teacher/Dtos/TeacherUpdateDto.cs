using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Teacher.Dtos
{
    public class TeacherUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string Department { get; set; }
    }
}
