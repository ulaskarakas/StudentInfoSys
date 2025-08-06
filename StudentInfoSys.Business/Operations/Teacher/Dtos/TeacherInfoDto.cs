using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Teacher.Dtos
{
    public class TeacherInfoDto
    {
        public int Id { get; set; }
        [Required]
        public required string Department { get; set; }
    }
}