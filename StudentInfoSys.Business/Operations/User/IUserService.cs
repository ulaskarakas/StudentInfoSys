using PatikaLMSCoreProject.Business.Types;
using StudentInfoSys.Business.Operations.User.Dtos;

namespace StudentInfoSys.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> RegisterAsync(UserRegisterDto userRegisterDto);
    }
}