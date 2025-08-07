using StudentInfoSys.Business.Operations.Course.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.Course
{
    public interface ICourseService
    {
        Task<ServiceMessage> CreateAsync(CourseCreateDto dto);
        Task<ServiceMessage<CourseInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(CourseUpdateDto dto);
        Task<ServiceMessage> DeleteByIdAsync(int id);
    }
}