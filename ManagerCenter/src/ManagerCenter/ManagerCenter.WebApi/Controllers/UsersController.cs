using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.WebApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NJsonSchema.Annotations;
using NSwag.Annotations;

namespace ManagerCenter.WebApi.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "usergateway")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserService userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService,)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(typeof(PaginationResult<ApplicationUser>))]
        public async Task<IActionResult> GetUsersAsync(int skip, int take, string search)
        {
            PaginationResult<ApplicationUser> result = await userService.GetUsers(skip, take, search).ConfigureAwait(false);
            return new JsonResult(result);
        }


        /// <summary>
        /// 通过id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(typeof(ApplicationUser))]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            ApplicationUser user = await userService.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            return new JsonResult(user);
        }

        /// <summary>
        /// 通过id删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByIdAsync(string id)
        {
            var user = await userService.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userService.DeleteUsersAsync(new List<ApplicationUser> { user }).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }


        /// <summary>
        /// 创建或者更新用户
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(typeof(string))]
        public async Task<IActionResult> CreateOrUpdateUserAsync([FromBody]CreateOrUpdateUserViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            DataResult<string> result;
            if (string.IsNullOrEmpty(viewModel.Id))
            {
                result = await userService.CreateUserAsync(new CreateUserDto
                {
                    Name = viewModel.Name,
                    UserName = viewModel.UserName,
                    Password = viewModel.Password
                }).ConfigureAwait(false);
            }
            else
            {
                var user = await userService.FindByIdAsync(viewModel.Id).ConfigureAwait(false);
                if(user == null)
                {
                    return NotFound();
                }

                user.Name = viewModel.Name;
                user.UserName = viewModel.UserName;
                user.Password = viewModel.Password;
                result = await userService.UpdateUserAsync(user).ConfigureAwait(false);
            }

            if (result.Succeeded)
            {
                return new JsonResult(result.Data);
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 部分更新用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="document"></param>
        ///// <returns></returns>
        //[HttpPatch("{userId}")]
        //public async Task<IActionResult> PatchUserAsync([FromServices] ApplicationDbContext applicationDbContext,
        //    [FromServices] IMapper mapper,
        //    string userId, JsonPatchDocument<CreateOrUpdateUserViewModel> document)
        //{
        //    var user = await applicationDbContext.Users.FirstOrDefaultAsync(e => e.Id == userId).ConfigureAwait(false);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    CreateOrUpdateUserViewModel dto = mapper.Map<CreateOrUpdateUserViewModel>(user);
        //    document.ApplyTo(dto);

        //    user = mapper.Map(dto, user);

        //    try
        //    {
        //        await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
        //        return Ok();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/roles")]
        public async Task<List<ApplicationRole>> GetRolesOfUserAsync(string userId)
        {
            var result = await userService.GetRolesOfUserByUserIdAsync(userId).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddToRolesAsync([FromBody]AddToRolesViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var user = await userService.FindByIdAsync(viewModel.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userService.AddToRolesAsync(user, viewModel.RoleIds).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }


        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/claims")]
        public async Task<List<ApplicationIdentityUserClaim>> GetUserClaimsAsync(string userId)
        {
            List<ApplicationIdentityUserClaim> result = await userService.GetUserClaimsAsync(userId).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 创建或者更新角色声明
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        [HttpPost("{userId}/claims")]
        public async Task<IActionResult> CreateOrUpdateUserClaimAsync(string userId, ApplicationIdentityUserClaim claim)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"'{nameof(userId)}' cannot be null or empty", nameof(userId));
            }

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var result = await userService.CreateOrUpdateUserClaimAsync(claim).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }



        /// <summary>
        /// 删除用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/claims/{claimId}")]
        public async Task<IActionResult> DeleteUserClaimAsync(string userId, int claimId)
        {
            var userClaim = await userService.GetUserClaimByIdAsync(userId, claimId).ConfigureAwait(false);
            if (userClaim == null)
            {
                return NotFound();
            }

            var result = await userService.DeleteUserClaimsAsync(new List<ApplicationIdentityUserClaim> { userClaim }).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 用户拥有的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/Permissons")]
        public Task<List<string>> GetPermissonsOfUserAsync(string userId)
        {
            return userService.GetPermissonsOfUserAsync(userId);
        }
    }
}
