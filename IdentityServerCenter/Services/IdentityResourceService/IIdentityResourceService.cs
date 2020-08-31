using IdentityServerCenter.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    public interface IIdentityResourceService
    {
       
        Task<Result> DeleteIdentityResourceAsync(int id);
        Task<Result> CreateOrUpdateIdentityResourceAsync(IdentityResourceDto dto);
     
        Task<IdentityResourceDto> GetIdentityResourceByIdAsync(int id);
        Task<PaginationResult<IdentityResourceDto>> GetIdentityResourcesAsync(int? skip, int? take, string search);
    }
}
