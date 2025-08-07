using StudentInfoSys.Business.Operations.Enrollment.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.Enrollment
{
    public interface IEnrollmentService
    {
        Task<ServiceMessage> CreateAsync(EnrollmentCreateDto dto);
        Task<ServiceMessage<EnrollmentInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(EnrollmentUpdateDto dto);
        Task<ServiceMessage> DeleteByIdAsync(int id);
    }
}