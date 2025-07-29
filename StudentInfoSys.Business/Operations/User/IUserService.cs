using StudentInfoSys.Business.Types;
using StudentInfoSys.Business.Operations.User.Dtos;

namespace StudentInfoSys.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<ServiceMessage<UserInfoDto>> LoginAsync(UserLoginDto userLoginDto);
    }
}