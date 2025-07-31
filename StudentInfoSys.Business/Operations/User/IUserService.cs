using StudentInfoSys.Business.Types;
using StudentInfoSys.Business.Operations.User.Dtos;

namespace StudentInfoSys.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<ServiceMessage<UserInfoDto>> LoginAsync(UserLoginDto userLoginDto);
        Task<ServiceMessage<UserInfoDto>> GetByIdAsync(int id);
        Task<ServiceMessage> UpdateByIdAsync(UserUpdateDto userUpdateDto);
        Task<ServiceMessage> DeleteByIdAsync(int id);
    }
}