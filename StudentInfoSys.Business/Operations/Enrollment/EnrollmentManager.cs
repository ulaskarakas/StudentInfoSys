using StudentInfoSys.Business.Operations.Enrollment.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Enrollment
{
    public class EnrollmentManager : IEnrollmentService
    {
        private readonly IRepository<EnrollmentEntity> _enrollmentRepository;
        private readonly IRepository<StudentEntity> _studentRepository;
        private readonly IRepository<CourseEntity> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentManager(IRepository<EnrollmentEntity> enrollmentRepository, IRepository<StudentEntity> studentRepository, IRepository<CourseEntity> courseRepository, IUnitOfWork unitOfWork)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        // Create
        public async Task<ServiceMessage> CreateAsync(EnrollmentCreateDto enrollmentCreateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var student = await _studentRepository.GetByIdAsync(enrollmentCreateDto.StudentId);
                if (student == null || student.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Student not found." };
                }

                var course = await _courseRepository.GetByIdAsync(enrollmentCreateDto.CourseId);
                if (course == null || course.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Course not found." };
                }

                var existingEnrollment = await _enrollmentRepository.GetSingleAsync(e => e.StudentId == enrollmentCreateDto.StudentId && e.CourseId == enrollmentCreateDto.CourseId && !e.IsDeleted);
                if (existingEnrollment != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Enrollment already exists for this student and course." };
                }

                var enrollment = new EnrollmentEntity
                {
                    StudentId = enrollmentCreateDto.StudentId,
                    CourseId = enrollmentCreateDto.CourseId,
                    AbsanceCount = enrollmentCreateDto.AbsanceCount,
                    Student = student,
                    Course = course,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _enrollmentRepository.AddAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage 
                { 
                    IsSucceed = true, 
                    Message = "Enrollment created successfully." 
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage 
                { 
                    IsSucceed = false, 
                    Message = $"An error occurred: {ex.Message}" 
                };
            }
        }

        // Read
        public async Task<ServiceMessage<EnrollmentInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null || enrollment.IsDeleted)
                {
                    return new ServiceMessage<EnrollmentInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Enrollment not found."
                    };
                }

                var enrollmentDto = new EnrollmentInfoDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    AbsanceCount = enrollment.AbsanceCount
                };

                return new ServiceMessage<EnrollmentInfoDto>
                {
                    IsSucceed = true,
                    Data = enrollmentDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<EnrollmentInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred while retrieving the enrollment: {ex.Message}"
                };
            }
        }

        // Update
        public async Task<ServiceMessage> UpdateByIdAsync(EnrollmentUpdateDto enrollmentUpdateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentUpdateDto.Id);
                if (enrollment == null || enrollment.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Enrollment not found."
                    };
                }

                enrollment.AbsanceCount = enrollmentUpdateDto.AbsanceCount;
                enrollment.ModifiedDate = DateTime.UtcNow;

                _enrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Enrollment updated successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while updating the enrollment: {ex.Message}"
                };
            }
        }

        // Delete
        public async Task<ServiceMessage> DeleteByIdAsync(int id)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(id);
                if (enrollment == null || enrollment.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Enrollment not found."
                    };
                }

                enrollment.IsDeleted = true;
                enrollment.ModifiedDate = DateTime.UtcNow;

                _enrollmentRepository.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Enrollment deleted successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while deleting the enrollment: {ex.Message}"
                };
            }
        }
    }
}