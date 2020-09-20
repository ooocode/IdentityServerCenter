using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.Abstractions.Models.UserManagerModels;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions.UserManagerInterfaces
{
    public interface IPermissonService
    {
        Task<DataResult<string>> CreateOrUpdatePermissonAsync(Permisson permisson);
        Task<Result> DeletePermissonAsync(string id);
        Task<Permisson> GetPermissonByIdAsync(string id);
        Task<PaginationResult<Permisson>> GetPermissonsAsync(int? skip, int? take, string search);
    }
}
