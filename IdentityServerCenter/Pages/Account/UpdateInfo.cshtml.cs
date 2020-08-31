using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace IdentityServerCenter.Pages.Account
{
    [IgnoreAntiforgeryToken]
    [Authorize]
    public class UpdateInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public UpdateInfoModel(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<string> SavaFileAsync(HttpContext httpContext)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            IFormFile file = httpContext.Request.Form.Files[0];
            if (file != null)
            {
                if (file.Length > 50 * 1024 * 1024)
                {
                    //errorMsg = "文件大小超过50MB";
                    return null;
                }

                ///获取扩展名
                var provider = new FileExtensionContentTypeProvider();
                var extName = provider.Mappings.FirstOrDefault(e => e.Value == file.ContentType).Key;
                string filename = DateTime.Now.Ticks.ToString() + "_" + Guid.NewGuid().ToString("N") + extName;

                var saveDir = configuration["AvatarSavePath"];

                string savePath = System.IO.Path.Combine(saveDir, filename);
                using (var fileStream = System.IO.File.Create(savePath))
                {
                    await file.OpenReadStream().CopyToAsync(fileStream).ConfigureAwait(false);
                }

                if (System.IO.File.Exists(savePath))
                {
                    return $"/api/files?name={filename}";
                }
            }
            return null;
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUploadPhotoAsync()
        {
            var filename = await SavaFileAsync(HttpContext).ConfigureAwait(false);
            if (string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }

            var user = await userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "无效的用户");
                return BadRequest(ModelState);
            }

            user.Photo = filename;

            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault()?.Description);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync(UpdateUserInfoViewModel updateModel)
        {
            if (updateModel is null)
            {
                throw new ArgumentNullException(nameof(updateModel));
            }

            if (!TryValidateModel(updateModel))
            {
                return BadRequest(ModelState);
            }


            var user = await userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "无效的用户");
                return BadRequest(ModelState);
            }

            user.Sex = updateModel.Sex;
            user.Desc = updateModel.Desc ?? string.Empty;
            user.Email = updateModel.Email ?? string.Empty;
            user.Name = updateModel.Name ?? string.Empty;
            user.PhoneNumber = updateModel.PhoneNumber ?? string.Empty;

            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault()?.Description);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }



        /// <summary>
        /// 更新密码
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdatePasswordAsync(UpdatePasswordViewModel updatePasswordDto)
        {
            if (updatePasswordDto is null)
            {
                throw new ArgumentNullException(nameof(updatePasswordDto));
            }

            if (!TryValidateModel(updatePasswordDto))
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "无效的用户");
                return BadRequest(ModelState);
            }

            user.Password = updatePasswordDto.NewPassword;

            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault()?.Description);
                return BadRequest(ModelState);
            }

            return RedirectToAction("Logout", "Account");
        }
    }

    /// <summary>
    /// 更新密码
    /// </summary>
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度6--20位")]
        [Compare("ConfirmPassword", ErrorMessage = "两次输入的密码不一致")]
        public string NewPassword { get; set; }


        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度6--20位")]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 更新信息视图模型
    /// </summary>
    public class UpdateUserInfoViewModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "姓名必须是3--30个字符")]
        public string Name { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "无效的邮箱格式")]
        public string Email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public byte Sex { get; set; }


        /// <summary>
        /// 个人简介
        /// </summary>
        public string Desc { get; set; }


        /// <summary>
        /// 班级id
        /// </summary>
        public string ClassId { get; set; }
    }
}