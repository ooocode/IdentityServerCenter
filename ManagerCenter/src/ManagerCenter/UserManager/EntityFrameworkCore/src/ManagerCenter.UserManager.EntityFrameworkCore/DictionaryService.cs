using AutoMapper;
using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class DictionaryService : ServiceBase, IDictionaryService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;

        public DictionaryService(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DictionaryDto> GetDictionaryByIdAsync(long id)
        {
            var dictionary = await applicationDbContext.Dictionaries.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            var result = mapper.Map<DictionaryDto>(dictionary);
            return result;
        }

        /// <summary>
        /// 获取词典数据
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<DictionaryDto>> QueryDictionariesAsync(long? typeId, string code, string remark)
        {
            IQueryable<Dictionary> directories = applicationDbContext.Dictionaries;
            if (typeId.HasValue)
            {
                directories = directories.Where(e => e.DictionaryTypeId == typeId);
            }

            if (!string.IsNullOrEmpty(code))
            {
                directories = directories.Where(e => e.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(remark))
            {
                directories = directories.Where(e => e.Remark.Contains(remark));
            }

            var ls = await directories.ToListAsync().ConfigureAwait(false);
            var result = mapper.Map<List<DictionaryDto>>(ls);
            return result;
        }

        /// <summary>
        /// 删除数据字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteDictionaryByIdAsync(long id)
        {
            var dictionary = await applicationDbContext.Dictionaries.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (dictionary == null)
            {
                return FailedResult("数据字典不存在");
            }

            applicationDbContext.Dictionaries.Remove(dictionary);
            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return FailedResult(ex);
            }
        }

        /// <summary>
        /// 创建或者更新字典
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<long>> CreateOrUpdateDictionaryAsync(CreateOrUpdateDictionaryDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var existType = await applicationDbContext.DictionaryTypes.AnyAsync(e => e.Id == dto.DictionaryTypeId).ConfigureAwait(false);
            if (!existType)
            {
                return FailedDataResult<long>("类型不存在");
            }

            if (dto.Id.HasValue)
            {
                var dictionary = await applicationDbContext.Dictionaries.FirstOrDefaultAsync(e => e.Id == dto.Id).ConfigureAwait(false);
                if (dictionary == null)
                {
                    return FailedDataResult<long>("数据字典不存在");
                }

                dictionary = mapper.Map(dto, dictionary);
            }
            else
            {
                dto.Id = GuidEx.NewGuid();
                Dictionary entity = mapper.Map<Dictionary>(dto);
                await applicationDbContext.Dictionaries.AddAsync(entity).ConfigureAwait(false);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkDataResult(dto.Id.Value);
            }
            catch (DbUpdateException ex)
            {
                return FailedDataResult<long>(ex);
            }
        }

        /// <summary>
        /// 获取目录类型
        /// </summary>
        /// <returns></returns>
        public async Task<List<DictionaryTypeDto>> GetDictionaryTypesAsync()
        {
            var ls = await applicationDbContext.DictionaryTypes.ToListAsync().ConfigureAwait(false);
            var result = mapper.Map<List<DictionaryTypeDto>>(ls);
            return result;
        }

        /// <summary>
        /// 获取目录类型通过id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DictionaryTypeDto> GetDictionaryTypeByIdAsync(long id)
        {
            var entity = await applicationDbContext.DictionaryTypes.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            var result = mapper.Map<DictionaryTypeDto>(entity);
            return result;
        }

        /// <summary>
        /// 创建或者更新字典类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<long>> CreateOrUpdateDictionaryTypeAsync(CreateOrUpdateDictionaryTypeDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (!dto.Id.HasValue)
            {
                dto.Id = GuidEx.NewGuid();
                DictionaryType entity = mapper.Map<DictionaryType>(dto);
                await applicationDbContext.DictionaryTypes.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                var model = await applicationDbContext.DictionaryTypes.FirstOrDefaultAsync(e => e.Id == dto.Id).ConfigureAwait(false);
                model = mapper.Map(dto, model);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkDataResult(dto.Id.Value);
            }
            catch (DbUpdateException ex)
            {
                return FailedDataResult<long>(ex);
            }
        }

        /// <summary>
        /// 删除字典类型（自动删除类型对应的字典）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteDictionaryTypeByIdAsync(long id)
        {
            var type = await applicationDbContext.DictionaryTypes.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (type == null)
            {
                return FailedResult(ErrorMessageConsts.ResourceNotFound);
            }

            var dictionariesOfType = applicationDbContext.Dictionaries.Where(e => e.DictionaryTypeId == id);
            applicationDbContext.Dictionaries.RemoveRange(dictionariesOfType);
            applicationDbContext.DictionaryTypes.Remove(type);

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }
    }
}
