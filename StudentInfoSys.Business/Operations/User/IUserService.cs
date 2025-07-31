using StudentInfoSys.Business.Operations.User.Dtos;
using StudentInfoSys.Business.Types;

namespace StudentInfoSys.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<ServiceMessage<UserInfoDto>> LoginAsync(UserLoginDto userLoginDto);
        Task<ServiceMessage<UserInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(UserUpdateDto userUpdateDto);
        Task<ServiceMessage> DeleteByIdAsync(int id);
        Task<ServiceMessage> AssignRoleAsync(string email, string roleName);
        Task<ServiceMessage> RemoveRoleAsync(string email, string roleName);
    }
}