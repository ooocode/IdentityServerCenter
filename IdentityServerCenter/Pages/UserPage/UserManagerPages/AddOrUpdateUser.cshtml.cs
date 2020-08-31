using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Models;
using IdentityServerCenter.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter
{

    public class AddOrUpdateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IUserService userService;


        public AddOrUpdateUserModel(UserManager<ApplicationUser> userManager,
            ApplicationDbContext applicationDbContext,
            IUserService userService)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }


        [FromRoute(Name = "Id")]
        public string Id { get; set; }


        public ApplicationUser ApplicationUser { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                ApplicationUser = new ApplicationUser();
            }
            else
            {
                ApplicationUser = await userService.FindByIdAsync(Id).ConfigureAwait(false);
                if (ApplicationUser == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        /// <summary>
        /// 创建或编辑用户
        /// </summary>
        /// <param name="addOrUpdateUserViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCreateOrUpdateUserAsync(CreateOrUpdateUserViewModel addOrUpdateUserViewModel)
        {
            if (addOrUpdateUserViewModel is null)
            {
                throw new ArgumentNullException(nameof(addOrUpdateUserViewModel));
            }

            if (!TryValidateModel(addOrUpdateUserViewModel))
            {
                return BadRequest(ModelState);
            }

            var result = await userService.CreateOrUpdateUserAsync(new Models.ApplicationUser
            {
                Id = addOrUpdateUserViewModel.Id,
                UserName = addOrUpdateUserViewModel.UserName,
                Name = addOrUpdateUserViewModel.Name,
                Password = addOrUpdateUserViewModel.Password
            }).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return Content(result.Data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return BadRequest(ModelState);
            }
        }


        /// <summary>
        /// 改变用户现有角色
        /// </summary>
        /// <param name="changeUserRolesViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostChangeUserRolesAsync(ChangeUserRolesViewModel changeUserRolesViewModel)
        {
            if (changeUserRolesViewModel is null)
            {
                throw new ArgumentNullException(nameof(changeUserRolesViewModel));
            }

            var user = await userManager.FindByIdAsync(changeUserRolesViewModel.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            //先移除用户所有角色
            var removeRoles = applicationDbContext.UserRoles.Where(e => e.UserId == user.Id);
            applicationDbContext.UserRoles.RemoveRange(removeRoles);

            //再添加角色
            IEnumerable<string> roleNames = changeUserRolesViewModel.Roles.FirstOrDefault()?.Split('、').Where(e => !string.IsNullOrEmpty(e));
            if (roleNames != null)
            {
                foreach (var name in roleNames)
                {
                    var r = applicationDbContext.Roles.FirstOrDefault(e => e.Name == name);
                    if (r != null)
                    {
                        applicationDbContext.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = r.Id });
                    }
                }

            }

            await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            return Content(user.Id);
        }



        public string UserClaims => "?handler=UserClaims";
        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetUserClaimsAsync(string userId)
        {
            var userClaims = await userService.GetUserClaimsAsync(userId).ConfigureAwait(false);
            return new JsonResult(userClaims);
        }


        public string CreateOrUpdateUserClaimPostUrl => "?handler=CreateOrUpdateUserClaim";

        /// <summary>
        /// 创建或者更新用户声明
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCreateOrUpdateUserClaimAsync(ApplicationIdentityUserClaim claim)
        {
            if (!TryValidateModel(claim))
            {
                return BadRequest(ModelState);
            }

            var result = await userService.CreateOrUpdateUserClaimAsync(claim).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new OkResult();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 删除用户声明
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteUserClaimAsync([FromBody] DeleteUserClaimViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!TryValidateModel(viewModel))
            {
                return BadRequest(ModelState);
            }

            var result = await userService.DeleteUserClaimAsync(viewModel.UserId, viewModel.Id).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }
    }



    public class ChangeUserRolesViewModel
    {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }

    public class DeleteUserClaimViewModel
    {
        public string UserId { get; set; }
        public int Id { get; set; }
    }
}