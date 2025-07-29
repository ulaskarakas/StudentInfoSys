using StudentInfoSys.Business.Types;
using StudentInfoSys.Business.DataProtection;
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
        private readonly IDataProtection _dataProtection;

        public UserManager(IRepository<UserEntity> userRepository, IRepository<RoleEntity> roleRepository, IRepository<UserRoleEntity> userRoleRepository, IUnitOfWork unitOfWork, IDataProtection dataProtection)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
            _dataProtection = dataProtection;
        }

        public async Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var existingUser = await _userRepository.GetSingleAsync(u => u.Email == userRegisterDto.Email && !u.IsDeleted);
                if (existingUser != null)
                {
                    await _unitOfWork.RollbackTransaction();
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
                    Password = _dataProtection.Protect(userRegisterDto.Password),
                    BirthDate = userRegisterDto.BirthDate,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _userRepository.AddAsync(userEntity);
                await _unitOfWork.SaveChangesAsync();

                var defaultRole = await _roleRepository.GetSingleAsync(r => r.Name == "User" && !r.IsDeleted);
                if (defaultRole == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Default user role not found. Registration failed."
                    };
                }

                var userRole = new UserRoleEntity
                {
                    UserId = userEntity.Id,
                    RoleId = defaultRole.Id
                };
                await _userRoleRepository.AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred during user registration: {ex.Message}"
                };
            }
        }

        public async Task<ServiceMessage<UserInfoDto>> LoginAsync(UserLoginDto userLoginDto)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(u => u.Email == userLoginDto.Email && !u.IsDeleted);

                if (user == null)
                {
                    return new ServiceMessage<UserInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Invalid email or password."
                    };
                }

                var decryptedPassword = _dataProtection.Unprotect(user.Password);

                if (decryptedPassword != userLoginDto.Password)
                {
                    return new ServiceMessage<UserInfoDto>
                    {
                        IsSucceed = false,
                        Message = "Invalid email or password."
                    };
                }

                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        BirthDate = user.BirthDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred during login: {ex.Message}"
                };
            }
        }
    }
}
