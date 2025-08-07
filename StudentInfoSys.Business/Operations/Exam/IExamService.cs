using StudentInfoSys.Business.Operations.Exam.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.Exam
{
    public interface IExamService
    {
        Task<ServiceMessage> CreateAsync(ExamCreateDto dto);
        Task<ServiceMessage<ExamInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(ExamUpdateDto dto);
        Task<ServiceMessage> DeleteByIdAsync(int id);
    }
}
