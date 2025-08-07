using StudentInfoSys.Business.Operations.Teacher.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Teacher
{
    public class TeacherManager : ITeacherService
    {
        private readonly IRepository<TeacherEntity> _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TeacherManager(IRepository<TeacherEntity> teacherRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        // Read
        public async Task<ServiceMessage<TeacherInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var teacher = await _teacherRepository.GetSingleAsync(t => t.Id == id && !t.IsDeleted);
                if (teacher == null) 
                {
                    return new ServiceMessage<TeacherInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Teacher not found."
                    };
                }

                var teacherDto = new TeacherInfoDto
                {
                    Id = teacher.Id,
                    Department = teacher.Department
                };

                return new ServiceMessage<TeacherInfoDto>
                {
                    IsSucceed = true,
                    Data = teacherDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<TeacherInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred while retrieving the teacher: {ex.Message}"
                };
            }
        }

        // Update
        public async Task<ServiceMessage> UpdateByIdAsync(TeacherUpdateDto teacherUpdateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var teacher = await _teacherRepository.GetByIdAsync(teacherUpdateDto.Id);
                if (teacher == null || teacher.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Teacher not found."
                    };
                }

                teacher.Department = teacherUpdateDto.Department;
                teacher.ModifiedDate = DateTime.UtcNow;

                _teacherRepository.Update(teacher);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Teacher updated successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while updating the teacher: {ex.Message}"
                };
            }
        }
    }
}