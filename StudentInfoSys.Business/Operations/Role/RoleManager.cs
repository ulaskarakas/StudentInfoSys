using PatikaLMSCoreProject.Business.Types;
using StudentInfoSys.Business.Operations.Role.Dtos;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Role
{
    public class RoleManager : IRoleService
    {
        private readonly IRepository<RoleEntity> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleManager(IRepository<RoleEntity> roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceMessage> CreateAsync(RoleCreateDto roleCreateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var existingRole = await _roleRepository.GetSingleAsync(r => r.Name == roleCreateDto.Name && !r.IsDeleted);
                if (existingRole != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "A role with this name already exists."
                    };
                }

                var roleEntity = new RoleEntity
                {
                    Name = roleCreateDto.Name,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _roleRepository.AddAsync(roleEntity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "Role created successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while creating the role: {ex.Message}"
                };
            }
        }
    }
}