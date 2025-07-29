using StudentInfoSys.Business.Types;
using StudentInfoSys.Business.Operations.Role.Dtos;

namespace StudentInfoSys.Business.Operations.Role
{
    public interface IRoleService
    {
        Task<ServiceMessage> CreateAsync(RoleCreateDto roleCreateDto);
    }
}