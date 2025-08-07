using System.ComponentModel.DataAnnotations;

namespace StudentInfoSys.WebApi.Models.Enrollment
{
    public class EnrollmentUpdateRequest
    {
        [Required]
        public required int AbsanceCount { get; set; }
    }
}