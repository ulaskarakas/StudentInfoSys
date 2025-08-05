using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.Business.Operations.Student.Dtos
{
    public class StudentInfoDto
    {
        public int Id { get; set; }
        [Required]
        public required string StudentNumber { get; set; }
        [Required]
        public required string Class { get; set; }
    }
}
