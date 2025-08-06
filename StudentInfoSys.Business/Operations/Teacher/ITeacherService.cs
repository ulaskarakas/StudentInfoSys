using StudentInfoSys.Business.Operations.Teacher.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.Teacher
{
    public interface ITeacherService
    {
        Task<ServiceMessage<TeacherInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(TeacherUpdateDto dto);
    }
}