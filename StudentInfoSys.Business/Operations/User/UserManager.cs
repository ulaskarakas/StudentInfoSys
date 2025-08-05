using Microsoft.EntityFrameworkCore;
using StudentInfoSys.Business.DataProtection;
using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.Business.Types;
using StudentInfoSys.Data.Context;
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
        private readonly IRepository<StudentEntity> _studentRepository;
        private readonly IRepository<TeacherEntity> _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtection _dataProtection;
        private readonly StudentInfoSysDbContext _context;

        public UserManager(IRepository<UserEntity> userRepository, IRepository<RoleEntity> roleRepository, IRepository<UserRoleEntity> userRoleRepository, IRepository<StudentEntity> studentRepository, IRepository<TeacherEntity> teacherRepository, IUnitOfWork unitOfWork, IDataProtection dataProtection, StudentInfoSysDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _dataProtection = dataProtection;
            _context = context;
        }

        // Register
        public async Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto.RoleNames == null || !userRegisterDto.RoleNames.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "At least one role must be selected for the user."
                };
            }

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

                foreach (var roleName in userRegisterDto.RoleNames)
                {
                    var role = await _roleRepository.GetSingleAsync(r => r.Name == roleName && !r.IsDeleted);
                    if (role == null)
                    {
                        await _unitOfWork.RollbackTransaction();
                        return new ServiceMessage
                        {
                            IsSucceed = false,
                            Message = $"Role '{roleName}' not found. Registration failed."
                        };
                    }

                    // Soft delete kontrolü
                    var existingUserRole = await _userRoleRepository.GetSingleAsync(
                        ur => ur.UserId == userEntity.Id && ur.RoleId == role.Id);

                    if (existingUserRole != null)
                    {
                        if (existingUserRole.IsDeleted)
                        {
                            existingUserRole.IsDeleted = false;
                            existingUserRole.ModifiedDate = DateTime.UtcNow;
                            _userRoleRepository.Update(existingUserRole);
                        }
                    }
                    else
                    {
                        var userRole = new UserRoleEntity
                        {
                            UserId = userEntity.Id,
                            RoleId = role.Id,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _userRoleRepository.AddAsync(userRole);
                    }
                }

                // Öğrenci veya öğretmen tablosuna ekleme
                if (userRegisterDto.RoleNames.Contains("Student"))
                {
                    var existingStudent = await _studentRepository.GetSingleAsync(s => s.UserId == userEntity.Id);
                    if (existingStudent != null)
                    {
                        if (existingStudent.IsDeleted)
                        {
                            existingStudent.IsDeleted = false;
                            existingStudent.ModifiedDate = DateTime.UtcNow;
                            _studentRepository.Update(existingStudent);
                        }
                    }
                    else
                    {
                        var studentEntity = new StudentEntity
                        {
                            UserId = userEntity.Id,
                            StudentNumber = string.Empty,
                            Class = string.Empty,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false,
                            User = userEntity
                        };
                        await _studentRepository.AddAsync(studentEntity);
                    }
                }

                if (userRegisterDto.RoleNames.Contains("Teacher"))
                {
                    var existingTeacher = await _teacherRepository.GetSingleAsync(t => t.UserId == userEntity.Id);
                    if (existingTeacher != null)
                    {
                        if (existingTeacher.IsDeleted)
                        {
                            existingTeacher.IsDeleted = false;
                            existingTeacher.ModifiedDate = DateTime.UtcNow;
                            _teacherRepository.Update(existingTeacher);
                        }
                    }
                    else
                    {
                        var teacherEntity = new TeacherEntity
                        {
                            UserId = userEntity.Id,
                            Department = string.Empty,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false,
                            User = userEntity
                        };
                        await _teacherRepository.AddAsync(teacherEntity);
                    }
                }

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
            // RoleNames listesi boşsa hata döndür
            if (userUpdateDto.RoleNames == null || !userUpdateDto.RoleNames.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "At least one role must be selected for the user."
                };
            }

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

                // Kullanıcının mevcut rollerini sil
                var existingRoles = await _userRoleRepository.GetWhereAsync(ur => ur.UserId == user.Id && !ur.IsDeleted);
                foreach (var userRole in existingRoles)
                {
                    userRole.IsDeleted = true;
                    userRole.ModifiedDate = DateTime.UtcNow;
                    _userRoleRepository.Update(userRole);
                }

                // Yeni roller ekle
                foreach (var roleName in userUpdateDto.RoleNames)
                {
                    var role = await _roleRepository.GetSingleAsync(r => r.Name == roleName && !r.IsDeleted);
                    if (role == null)
                    {
                        await _unitOfWork.RollbackTransaction();
                        return new ServiceMessage
                        {
                            IsSucceed = false,
                            Message = $"Role '{roleName}' not found. Update failed."
                        };
                    }

                    var existingUserRole = await _userRoleRepository.GetSingleAsync(
                        ur => ur.UserId == user.Id && ur.RoleId == role.Id);

                    if (existingUserRole != null)
                    {
                        if (existingUserRole.IsDeleted)
                        {
                            existingUserRole.IsDeleted = false;
                            existingUserRole.ModifiedDate = DateTime.UtcNow;
                            _userRoleRepository.Update(existingUserRole);
                        }
                    }
                    else
                    {
                        var newUserRole = new UserRoleEntity
                        {
                            UserId = user.Id,
                            RoleId = role.Id,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        await _userRoleRepository.AddAsync(newUserRole);
                    }
                }

                // --- Student/Teacher tablosu güncelleme ---
                var isStudentRole = userUpdateDto.RoleNames.Contains("Student");
                var studentEntity = await _context.Students.IgnoreQueryFilters()
                                                           .FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (studentEntity != null)
                {
                    if (!isStudentRole && !studentEntity.IsDeleted)
                    {
                        studentEntity.IsDeleted = true;
                        studentEntity.ModifiedDate = DateTime.UtcNow;
                        _studentRepository.Update(studentEntity);
                    }
                    else if (isStudentRole && studentEntity.IsDeleted)
                    {
                        studentEntity.IsDeleted = false;
                        studentEntity.ModifiedDate = DateTime.UtcNow;
                        _studentRepository.Update(studentEntity);
                    }
                }
                else if (isStudentRole)
                {
                    var newStudent = new StudentEntity
                    {
                        UserId = user.Id,
                        StudentNumber = string.Empty,
                        Class = string.Empty,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false,
                        User = user
                    };
                    await _studentRepository.AddAsync(newStudent);
                }

                var isTeacherRole = userUpdateDto.RoleNames.Contains("Teacher");
                var teacherEntity = await _context.Teachers.IgnoreQueryFilters()
                                                           .FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacherEntity != null)
                {
                    if (!isTeacherRole && !teacherEntity.IsDeleted)
                    {
                        teacherEntity.IsDeleted = true;
                        teacherEntity.ModifiedDate = DateTime.UtcNow;
                        _teacherRepository.Update(teacherEntity);
                    }
                    else if (isTeacherRole && teacherEntity.IsDeleted)
                    {
                        teacherEntity.IsDeleted = false;
                        teacherEntity.ModifiedDate = DateTime.UtcNow;
                        _teacherRepository.Update(teacherEntity);
                    }
                }
                else if (isTeacherRole)
                {
                    var newTeacher = new TeacherEntity
                    {
                        UserId = user.Id,
                        Department = string.Empty,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false,
                        User = user
                    };
                    await _teacherRepository.AddAsync(newTeacher);
                }

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

                // User'a bağlı StudentEntity'yi soft-delete et
                var studentEntity = await _context.Students
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(s => s.UserId == id);
                if (studentEntity != null && !studentEntity.IsDeleted)
                {
                    studentEntity.IsDeleted = true;
                    studentEntity.ModifiedDate = DateTime.UtcNow;
                    _studentRepository.Update(studentEntity);
                }

                // User'a bağlı TeacherEntity'yi soft-delete et
                var teacherEntity = await _context.Teachers
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.UserId == id);
                if (teacherEntity != null && !teacherEntity.IsDeleted)
                {
                    teacherEntity.IsDeleted = true;
                    teacherEntity.ModifiedDate = DateTime.UtcNow;
                    _teacherRepository.Update(teacherEntity);
                }

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
    }
}