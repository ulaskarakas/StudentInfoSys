using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentInfoSys.Business.Operations.User.Dtos
{
    public class UserRoleRemoveRequestDto
    {
        public required string Email { get; set; }
        public required string RoleName { get; set; }
    }
}
