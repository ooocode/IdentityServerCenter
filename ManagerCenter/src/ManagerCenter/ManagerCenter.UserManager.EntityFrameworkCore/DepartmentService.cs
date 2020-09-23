using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions.UserManagerInterfaces;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext applicationDbContext;

        public DepartmentService(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public Task<DataResult<string>> CreateDepartment()
        {
            applicationDbContext.Departments.AddAsync()
        }
    }
}
