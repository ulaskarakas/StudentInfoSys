using PatikaLMSCoreProject.Business.Types;
using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<RoleEntity> _roleRepository;
        private readonly IRepository<UserRoleEntity> _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserManager(IRepository<UserEntity> userRepository, IRepository<RoleEntity> roleRepository, IRepository<UserRoleEntity> userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            var existingUser = await _userRepository.GetSingleAsync(u => u.Email == userRegisterDto.Email && !u.IsDeleted);
            if (existingUser != null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "User with this email already exists."
                };
            }

            var userEntity = new UserEntity
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                Email = userRegisterDto.Email,
                Password = userRegisterDto.Password, // TODO: Hash the password before saving
                BirthDate = userRegisterDto.BirthDate,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            await _userRepository.AddAsync(userEntity);

            var defaultRole = await _roleRepository.GetSingleAsync(r => r.Name == "User" && !r.IsDeleted);
            if (defaultRole == null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Default user role not found. Registration failed."
                };
            }
            else 
            {
                var userRole = new UserRoleEntity
                {
                    UserId = userEntity.Id,
                    RoleId = defaultRole.Id
                };
                await _userRoleRepository.AddAsync(userRole);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred during user registration: {ex.Message}"
                };
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "User registered successfully."
            };

        }
    }
}
