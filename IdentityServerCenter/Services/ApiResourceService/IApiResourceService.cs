using IdentityServerCenter.Identity;
using IdentityServerCenter.Services.IdentityResourceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ApiResourceService
{
    public interface IApiResourceService
    {
        Task<Result> CreateOrUpdateApiResourceAsync(ApiResourceDto dto);
    
        Task<Result> DeleteApiResourceAsync(int id);

        Task<ApiResourceDto> GetApiResourceByIdAsync(int id);

        Task<PaginationResult<ApiResourceDto>> GetApiResourcesAsync(int? skip, int? take, string search);
    }
}
