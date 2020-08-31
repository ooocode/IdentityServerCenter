using IdentityServerCenter.Database.Dtos.DictionaryDtos;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerCenter.Database.Services
{
    public interface IDictionaryService
    {
        Task<DataResult<long>> CreateOrUpdateDictionaryAsync(CreateOrUpdateDictionaryDto dto);
        Task<DataResult<long>> CreateOrUpdateDictionaryTypeAsync(CreateOrUpdateDictionaryTypeDto dto);
        Task<Result> DeleteDictionaryByIdAsync(long id);
        Task<Result> DeleteDictionaryTypeByIdAsync(long id);
        Task<DictionaryDto> GetDictionaryByIdAsync(long id);
        Task<DictionaryTypeDto> GetDictionaryTypeByIdAsync(long id);
        Task<List<DictionaryTypeDto>> GetDictionaryTypesAsync();
        Task<List<DictionaryDto>> QueryDictionariesAsync(long? typeId = null, string code = null, string remark = null);
    }
}
