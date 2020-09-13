using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions
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
