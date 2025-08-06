using StudentInfoSys.Business.Operations.Student.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Student
{
    public class StudentManager : IStudentService
    {
        private readonly IRepository<StudentEntity> _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentManager(IRepository<StudentEntity> studentRepository, IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceMessage<StudentInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var student = await _studentRepository.GetSingleAsync(u => u.Id == id && !u.IsDeleted);
                if (student == null)
                {
                    return new ServiceMessage<StudentInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Student not found."
                    };
                }

                var studentDto = new StudentInfoDto
                {
                    Id = student.Id,
                    StudentNumber = student.StudentNumber,
                    Class = student.Class
                };

                return new ServiceMessage<StudentInfoDto>
                {
                    IsSucceed = true,
                    Data = studentDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<StudentInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred while retrieving the student: {ex.Message}"
                };
            }
        }

        public async Task<ServiceMessage> UpdateByIdAsync(StudentUpdateDto studentUpdateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentUpdateDto.Id);
                if (student == null || student.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Student not found."
                    };
                }

                var studentNumberExists = await _studentRepository.GetSingleAsync(u => u.StudentNumber == studentUpdateDto.StudentNumber && u.Id != studentUpdateDto.Id && !u.IsDeleted);
                if (studentNumberExists != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Another student with this student number already exists."
                    };
                }

                student.StudentNumber = studentUpdateDto.StudentNumber;
                student.Class = studentUpdateDto.Class;
                student.ModifiedDate = DateTime.UtcNow;

                _studentRepository.Update(student);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Student updated successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while updating the student: {ex.Message}"
                };
            }
        }
    }
}
