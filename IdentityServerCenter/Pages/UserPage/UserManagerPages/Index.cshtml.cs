using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerCenter.Pages.UserPage.UserManagerPages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IUserService userService;

        public IndexModel(IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IActionResult OnGet()
        {
            //return Redirect("/swagger/index.html");
            return Page();
        }

        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLoadUsersAsync(int? pageIndex, string search)
        {
            if (!pageIndex.HasValue || pageIndex == 0)
            {
                pageIndex = 1;
            }

            //每一页大小
            int PageSize = 10;
            int skip = (pageIndex.Value - 1) * PageSize;

            PaginationResult<ApplicationUser> result = await userService.GetUsers(skip, PageSize, search).ConfigureAwait(false);
          
            var rows = result.Rows.Select(e => new { User = e, Roles = userService.GetRolesOfUserByUserIdAsync(e.Id).Result });
            return new JsonResult(new { Total = result.Total, PageSize = PageSize, Rows = rows });
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="deleteUserViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteUserAsync([FromBody]DeleteUserViewModel deleteUserViewModel)
        {
            if (deleteUserViewModel is null)
            {
                throw new ArgumentNullException(nameof(deleteUserViewModel));
            }

            if (!TryValidateModel(deleteUserViewModel))
            {
                return BadRequest(ModelState);
            }

            var user = await userService.FindByIdAsync(deleteUserViewModel.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            //确认，防止乱删除
            if (!deleteUserViewModel.UserName.Trim().Equals(user.UserName,StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, $"{nameof(deleteUserViewModel.UserName)}输入错误");
                return BadRequest(ModelState);
            }

            var result = await userService.DeleteUserAsync(deleteUserViewModel.UserId).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new OkResult();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }


    /// <summary>
    /// 删除用户视图模型
    /// </summary>
    public class DeleteUserViewModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName  { get; set; }
    }
}