using ManagerCenter.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions.UserManagerInterfaces
{
    public interface IDepartmentService
    {
       Task<DataResult<string>> CreateDepartment(CreateDepartmentDto dto);
    }
}
