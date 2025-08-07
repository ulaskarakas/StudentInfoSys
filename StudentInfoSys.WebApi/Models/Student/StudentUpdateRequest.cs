using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Student
{
    public class StudentUpdateRequest
    {
        [Required]
        public required string StudentNumber { get; set; }
        [Required]
        public required string Class { get; set; }
    }
}