using StudentInfoSys.Business.Operations.Exam.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Exam
{
    public class ExamManager : IExamService
    {
        private readonly IRepository<ExamEntity> _examRepository;
        private readonly IRepository<EnrollmentEntity> _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamManager(
            IRepository<ExamEntity> examRepository,
            IRepository<EnrollmentEntity> enrollmentRepository,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        // Create
        public async Task<ServiceMessage> CreateAsync(ExamCreateDto examCreateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(examCreateDto.EnrollmentId);
                if (enrollment == null || enrollment.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Enrollment not found." };
                }

                // Check that no more than 3 exams are created for the same EnrollmentId in the same year
                var examYear = examCreateDto.ExamDate.Year;
                var examCount = await _examRepository.CountAsync(
                    e => e.EnrollmentId == examCreateDto.EnrollmentId
                      && !e.IsDeleted
                      && e.ExamDate.Year == examYear);

                if (examCount >= 3)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "No more than 3 exams can be created for the same enrollment in the same year."
                    };
                }

                var exam = new ExamEntity
                {
                    Title = examCreateDto.Title,
                    ExamDate = examCreateDto.ExamDate,
                    Grade = examCreateDto.Grade,
                    Enrollment = enrollment,
                    EnrollmentId = examCreateDto.EnrollmentId,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _examRepository.AddAsync(exam);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage 
                { 
                    IsSucceed = true, 
                    Message = "Exam created successfully." 
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
        public async Task<ServiceMessage<ExamInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(id);
                if (exam == null || exam.IsDeleted)
                {
                    return new ServiceMessage<ExamInfoDto> { IsSucceed = false, Message = "Exam not found." };
                }

                var examInfoDto = new ExamInfoDto
                {
                    Id = exam.Id,
                    Title = exam.Title,
                    ExamDate = exam.ExamDate,
                    Grade = exam.Grade,
                    EnrollmentId = exam.EnrollmentId
                };

                return new ServiceMessage<ExamInfoDto> 
                { 
                    IsSucceed = true, 
                    Data = examInfoDto 
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<ExamInfoDto> 
                { 
                    IsSucceed = false, 
                    Message = $"An error occurred: {ex.Message}" 
                };
            }
        }

        // Update
        public async Task<ServiceMessage> UpdateByIdAsync(ExamUpdateDto dto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var exam = await _examRepository.GetByIdAsync(dto.Id);
                if (exam == null || exam.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage 
                    { 
                        IsSucceed = false, 
                        Message = "Exam not found." 
                    };
                }

                exam.Title = dto.Title;
                exam.ExamDate = dto.ExamDate;
                exam.Grade = dto.Grade;
                exam.ModifiedDate = DateTime.UtcNow;

                _examRepository.Update(exam);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage 
                { 
                    IsSucceed = true, 
                    Message = "Exam updated successfully."
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

        // Delete
        public async Task<ServiceMessage> DeleteByIdAsync(int id)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var exam = await _examRepository.GetByIdAsync(id);
                if (exam == null || exam.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage 
                    { 
                        IsSucceed = false, 
                        Message = "Exam not found." 
                    };
                }

                exam.IsDeleted = true;
                exam.ModifiedDate = DateTime.UtcNow;

                _examRepository.Update(exam);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage 
                { 
                    IsSucceed = true, 
                    Message = "Exam deleted successfully." 
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
    }
}
