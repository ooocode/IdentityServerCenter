using IdentityServerCenter.Identity;
using IdentityServerCenter.Models;
using System.Threading.Tasks;

namespace IdentityServerCenter.Database.Services
{
    public interface IPermissonService
    {
        Task<DataResult<string>> CreateOrUpdatePermissonAsync(Permisson permisson);
        Task<Result> DeletePermissonAsync(string id);
        Task<Permisson> GetPermissonByIdAsync(string id);
        Task<PaginationResult<Permisson>> GetPermissonsAsync(int? skip, int? take, string search);
    }
}
