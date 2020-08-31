//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using IdentityServerCenter.Database.Dtos.DictionaryDtos;
//using IdentityServerCenter.Database.Services;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace IdentityServerCenter.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DictionariesController : ControllerBase
//    {
//        private readonly IDictionaryService dictionaryService;

//        public DictionariesController(IDictionaryService dictionaryService)
//        {
//            this.dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));
//        }

//        /// <summary>
//        /// 查询字典
//        /// </summary>
//        /// <param name="typeId"></param>
//        /// <param name="code"></param>
//        /// <param name="remark"></param>
//        /// <returns></returns>
//        [HttpGet]
//        public async Task<IEnumerable<DictionaryViewModel>> QueryDictionariesAsync(string typeId = null, string code = null, string remark = null)
//        {
//            long? longTypeId = null;
//            if (long.TryParse(typeId, out long temp))
//            {
//                longTypeId = temp;
//            }

//            var ls = await dictionaryService.QueryDictionariesAsync(longTypeId, code, remark).ConfigureAwait(false);
//            return ls.Select(e => new DictionaryViewModel
//            {
//                Id = e.Id.ToString(),
//                Code = e.Code,
//                DictionaryTypeId = e.DictionaryTypeId.ToString(),
//                Enabled = e.Enabled,
//                Remark = e.Remark,
//                Value = e.Value
//            });
//        }

//        /// <summary>
//        /// 根据id获取字典数据
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpGet("{id}")]
//        public async Task<DictionaryViewModel> GetDictionaryByIdAsync(string id)
//        {
//            if (long.TryParse(id, out long longId))
//            {
//                var e = await dictionaryService.GetDictionaryByIdAsync(longId).ConfigureAwait(false);
//                if (e == null)
//                {
//                    return null;
//                }

//                return new DictionaryViewModel
//                {
//                    Id = e.Id.ToString(),
//                    Code = e.Code,
//                    DictionaryTypeId = e.DictionaryTypeId.ToString(),
//                    Enabled = e.Enabled,
//                    Remark = e.Remark,
//                    Value = e.Value
//                };
//            }
//            return null;
//        }

//        /// <summary>
//        /// 创建或者更新字典
//        /// </summary>
//        /// <param name="dto"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<IActionResult> CreateOrUpdateDictionaryAsync(CreateOrUpdateDictionaryViewModel viewModel)
//        {
//            CreateOrUpdateDictionaryDto dto = new CreateOrUpdateDictionaryDto()
//            {
//                Code = viewModel.Code,
//                Remark = viewModel.Remark,
//                Enabled = viewModel.Enabled,
//                Value = viewModel.Value
//            };

//            if (string.IsNullOrEmpty(viewModel.Id))
//            {
//                dto.Id = null;
//            }
//            else
//            {
//                if (long.TryParse(viewModel.Id, out long longId))
//                {
//                    dto.Id = longId;
//                }
//            }

//            if (string.IsNullOrEmpty(viewModel.DictionaryTypeId))
//            {
//                dto.DictionaryTypeId = 0;
//            }
//            else
//            {
//                if (long.TryParse(viewModel.DictionaryTypeId, out long longId))
//                {
//                    dto.DictionaryTypeId = longId;
//                }
//            }


//            var result = await dictionaryService.CreateOrUpdateDictionaryAsync(dto).ConfigureAwait(false);
//            if (result.Succeeded)
//            {
//                return Ok();
//            }

//            ModelState.AddModelError(string.Empty, result.ErrorMessage);
//            return BadRequest(ModelState);
//        }


//        /// <summary>
//        /// 删除字典
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteDictionaryByIdAsync(string id)
//        {
//            if (long.TryParse(id, out long longId))
//            {
//                var result = await dictionaryService.DeleteDictionaryByIdAsync(longId).ConfigureAwait(false);
//                if (result.Succeeded)
//                {
//                    return Ok();
//                }

//                ModelState.AddModelError(string.Empty, result.ErrorMessage);
//            }
//            else
//            {
//                ModelState.AddModelError(string.Empty, "无效的Id");
//            }

//            return BadRequest(ModelState);
//        }


//        /// <summary>
//        /// 创建或者更新字典类型
//        /// </summary>
//        /// <param name="dto"></param>
//        /// <returns></returns>
//        [HttpPost("types")]
//        public async Task<IActionResult> CreateOrUpdateDictionaryTypeAsync(CreateOrUpdateDictionaryTypeViewModel viewModel)
//        {
//            CreateOrUpdateDictionaryTypeDto dto = new CreateOrUpdateDictionaryTypeDto()
//            {
//                Name = viewModel.Name,
//                Remark = viewModel.Remark
//            };
//            if (string.IsNullOrEmpty(viewModel.Id))
//            {
//                dto.Id = null;
//            }
//            else
//            {
//                if (long.TryParse(viewModel.Id, out long longId))
//                {
//                    dto.Id = longId;
//                }
//            }

//            var result = await dictionaryService.CreateOrUpdateDictionaryTypeAsync(dto).ConfigureAwait(false);
//            if (result.Succeeded)
//            {
//                return Ok();
//            }

//            ModelState.AddModelError(string.Empty, result.ErrorMessage);
//            return BadRequest(ModelState);
//        }


//        /// <summary>
//        /// 字典类型
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("types")]
//        public async Task<IEnumerable<DictionaryTypeViewModel>> GetDictionaryTypesAsync()
//        {
//            var ls = await dictionaryService.GetDictionaryTypesAsync().ConfigureAwait(false);
//            return ls.Select(e => new DictionaryTypeViewModel { Id = e.Id.ToString(), Name = e.Name, Remark = e.Remark });
//        }

//        /// <summary>
//        /// 根据id获取字典类型
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpGet("types/{id}")]
//        public async Task<DictionaryTypeViewModel> GetDictionaryTypeByIdAsync(string id)
//        {
//            if (long.TryParse(id, out long longId))
//            {
//                var type = await dictionaryService.GetDictionaryTypeByIdAsync(longId).ConfigureAwait(false);
//                return new DictionaryTypeViewModel
//                {
//                    Id = type.Id.ToString(),
//                    Name = type.Name,
//                    Remark = type.Remark
//                };
//            }
//            return null;
//        }


//        /// <summary>
//        /// 删除字典类型
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpDelete("types/{id}")]
//        public async Task<IActionResult> DeleteDictionaryTypeByIdAsync(string id)
//        {
//            if (long.TryParse(id, out long longId))
//            {
//                var result = await dictionaryService.DeleteDictionaryTypeByIdAsync(longId).ConfigureAwait(false);
//                if (result.Succeeded)
//                {
//                    return Ok();
//                }

//                ModelState.AddModelError(string.Empty, result.ErrorMessage);
//            }
//            else
//            {
//                ModelState.AddModelError(string.Empty, "无效的Id");
//            }

//            return BadRequest(ModelState);
//        }
//    }

//    public class DictionaryViewModel : DictionaryDto
//    {
//        public string Id { get; set; }

//        public string DictionaryTypeId { get; set; }
//    }

//    public class CreateOrUpdateDictionaryViewModel : CreateOrUpdateDictionaryDto
//    {
//        public string Id { get; set; }
//        public string DictionaryTypeId { get; set; }
//    }

//    public class CreateOrUpdateDictionaryTypeViewModel : CreateOrUpdateDictionaryTypeDto
//    {
//        public string Id { get; set; }
//    }

//    public class DictionaryTypeViewModel : DictionaryTypeDto
//    {
//        public string Id { get; set; }
//    }
//}
