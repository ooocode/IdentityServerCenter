using IdentityServerCenter.Identity;
using IdentityServerCenter.Services.IdentityResourceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ApiResourceService
{
    public interface IApiScopeService
    {
        Task<Result> CreateOrUpdateApiScopeAsync(ApiScopeDto dto);
     
        Task<Result> DeleteApiScopeAsync(int id);
    
        Task<ApiScopeDto> GetApiScopeByIdAsync(int id);

        Task<PaginationResult<ApiScopeDto>> GetApiScopesAsync(int? skip, int? take, string search);
    }
}
