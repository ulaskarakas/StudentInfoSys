using StudentInfoSys.Business.Operations.Course.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Course
{
    public class CourseManager : ICourseService
    {
        private readonly IRepository<CourseEntity> _courseRepository;
        private readonly IRepository<TeacherEntity> _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CourseManager(IRepository<CourseEntity> courseRepository, IRepository<TeacherEntity> teacherRepository, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        // Create
        public async Task<ServiceMessage> CreateAsync(CourseCreateDto courseCreateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var existingCourse = await _courseRepository.GetSingleAsync(c => c.CourseCode == courseCreateDto.CourseCode && c.Title == courseCreateDto.Title && !c.IsDeleted);
                if (existingCourse != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "A course with the same code and title already exists."
                    };
                }

                var teacher = await _teacherRepository.GetByIdAsync(courseCreateDto.TeacherId);
                if (teacher == null || teacher.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage 
                    { 
                        IsSucceed = false, 
                        Message = "Teacher not found." 
                    };
                }

                var course = new CourseEntity
                {
                    CourseCode = courseCreateDto.CourseCode,
                    Title = courseCreateDto.Title,
                    TeacherId = courseCreateDto.TeacherId,
                    Teacher = teacher,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _courseRepository.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage 
                { 
                    IsSucceed = true, 
                    Message = "Course created successfully." 
                };

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage 
                { 
                    IsSucceed = false, 
                    Message = $"An error occurred during course creation: {ex.Message}" 
                };
            }
        }

        // Read
        public async Task<ServiceMessage<CourseInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var course = await _courseRepository.GetSingleAsync(c => c.Id == id && !c.IsDeleted);
                if (course == null)
                {
                    return new ServiceMessage<CourseInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Course not found."
                    };
                }

                var courseDto = new CourseInfoDto
                {
                    Id = course.Id,
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    TeacherId = course.TeacherId
                };

                return new ServiceMessage<CourseInfoDto>
                {
                    IsSucceed = true,
                    Data = courseDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<CourseInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred while retrieving the course: {ex.Message}"
                };
            }
        }

        // Update
        public async Task<ServiceMessage> UpdateByIdAsync(CourseUpdateDto courseUpdateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseUpdateDto.Id);
                if (course == null || course.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Course not found."
                    };
                }

                var existingCourse = await _courseRepository.GetSingleAsync(c => c.CourseCode == courseUpdateDto.CourseCode && c.Title == courseUpdateDto.Title && c.Id != courseUpdateDto.Id && !c.IsDeleted);

                if (existingCourse != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Another course with the same code and title already exists."
                    };
                }

                var teacher = await _teacherRepository.GetByIdAsync(courseUpdateDto.TeacherId);
                if (teacher == null || teacher.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Teacher not found."
                    };
                }

                course.CourseCode = courseUpdateDto.CourseCode;
                course.Title = courseUpdateDto.Title;
                course.TeacherId = courseUpdateDto.TeacherId;
                course.ModifiedDate = DateTime.UtcNow;

                _courseRepository.Update(course);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Course updated successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while updating the course: {ex.Message}"
                };
            }
        }

        // Delete
        public async Task<ServiceMessage> DeleteByIdAsync(int id)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var course = await _courseRepository.GetByIdAsync(id);
                if (course == null || course.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Course not found."
                    };
                }

                course.IsDeleted = true;
                course.ModifiedDate = DateTime.UtcNow;

                _courseRepository.Update(course);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Course deleted successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while deleting the course: {ex.Message}"
                };
            }
        }
    }
}