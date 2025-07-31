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

        // Register
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

        // Login
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

                var userRoles = await _userRoleRepository.GetWhereAsync(ur => ur.UserId == user.Id && !ur.IsDeleted);
                var roleNames = new List<string>();
                foreach (var userRole in userRoles)
                {
                    var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (role != null && !role.IsDeleted)
                        roleNames.Add(role.Name);
                }

                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        BirthDate = user.BirthDate,
                        Roles = roleNames
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

        // Get an user
        public async Task<ServiceMessage<UserInfoDto>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(u => u.Id == id && !u.IsDeleted);
                if (user == null)
                {
                    return new ServiceMessage<UserInfoDto>
                    {
                        IsSucceed = false,
                        Message = "User not found."
                    };
                }

                var userRoles = await _userRoleRepository.GetWhereAsync(ur => ur.UserId == user.Id && !ur.IsDeleted);
                var roleNames = new List<string>();
                foreach (var userRole in userRoles)
                {
                    var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (role != null && !role.IsDeleted)
                        roleNames.Add(role.Name);
                }

                var userDto = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Roles = roleNames
                };

                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        // Update an user
        public async Task<ServiceMessage> UpdateByIdAsync(UserUpdateDto userUpdateDto)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userRepository.GetByIdAsync(userUpdateDto.Id);
                if (user == null || user.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "User not found."
                    };
                }

                var emailExists = await _userRepository.GetSingleAsync(u => u.Email == userUpdateDto.Email && u.Id != userUpdateDto.Id && !u.IsDeleted);
                if (emailExists != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "Another user with this email already exists."
                    };
                }

                user.FirstName = userUpdateDto.FirstName;
                user.LastName = userUpdateDto.LastName;
                user.Email = userUpdateDto.Email;
                user.BirthDate = userUpdateDto.BirthDate;

                if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
                {
                    user.Password = _dataProtection.Protect(userUpdateDto.Password);
                }

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "User updated successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while updating the user: {ex.Message}"
                };
            }
        }

        // Delete an user
        public async Task<ServiceMessage> DeleteByIdAsync(int id)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || user.IsDeleted)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage
                    {
                        IsSucceed = false,
                        Message = "User not found."
                    };
                }

                user.IsDeleted = true;
                _userRepository.Update(user);

                var userRoles = await _userRoleRepository.GetWhereAsync(ur => ur.UserId == id && !ur.IsDeleted);
                foreach (var userRole in userRoles)
                {
                    userRole.IsDeleted = true;
                    _userRoleRepository.Update(userRole);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage
                {
                    IsSucceed = true,
                    Message = "User deleted successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = $"An error occurred while deleting the user: {ex.Message}"
                };
            }
        }

        // Assign a role
        public async Task<ServiceMessage> AssignRoleAsync(string email, string roleName)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userRepository.GetSingleAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "User not found." };
                }

                var role = await _roleRepository.GetSingleAsync(r => r.Name == roleName && !r.IsDeleted);
                if (role == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Role not found." };
                }

                // Kullanıcıda bu rol zaten var mı kontrol et
                var hasRole = await _userRoleRepository.GetSingleAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id && !ur.IsDeleted);
                if (hasRole != null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "User already has this role." };
                }

                var newUserRole = new UserRoleEntity
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _userRoleRepository.AddAsync(newUserRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage { IsSucceed = true, Message = "Role assigned to user." };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage { IsSucceed = false, Message = ex.Message };
            }
        }

        // Remove a role
        public async Task<ServiceMessage> RemoveRoleAsync(string email, string roleName)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var user = await _userRepository.GetSingleAsync(u => u.Email == email && !u.IsDeleted);
                if (user == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "User not found." };
                }

                var role = await _roleRepository.GetSingleAsync(r => r.Name == roleName && !r.IsDeleted);
                if (role == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "Role not found." };
                }

                var userRole = await _userRoleRepository.GetSingleAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id && !ur.IsDeleted);
                if (userRole == null)
                {
                    await _unitOfWork.RollbackTransaction();
                    return new ServiceMessage { IsSucceed = false, Message = "User does not have this role." };
                }

                userRole.IsDeleted = true;
                _userRoleRepository.Update(userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return new ServiceMessage { IsSucceed = true, Message = "Role removed from user." };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return new ServiceMessage { IsSucceed = false, Message = ex.Message };
            }
        }
    }
}
