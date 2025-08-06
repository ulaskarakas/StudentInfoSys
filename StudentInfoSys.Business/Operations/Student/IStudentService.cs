using StudentInfoSys.Business.Operations.Student.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.Student
{
    public interface IStudentService
    {
        Task<ServiceMessage<StudentInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(StudentUpdateDto dto);
    }
}