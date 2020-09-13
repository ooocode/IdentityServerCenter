// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.UserManager.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ManagerCenter.Shared;
    using ManagerCenter.UserManager.Abstractions.Dtos;
    using ManagerCenter.UserManager.Abstractions.Models;

    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto">dto</param>
        /// <returns>结果</returns>
        Task<Result> AddToRolesAsync(AddToRolesDto dto);

        /// <summary>
        /// 创建或者更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<DataResult<string>> CreateOrUpdateUserAsync(ApplicationUser user);


        Task<Result> CreateOrUpdateUserClaimAsync(ApplicationIdentityUserClaim claim);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> DeleteUserClaimAsync(string userId, int cliamId);
        Task<ApplicationUser> FindByIdAsync(string id);
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task<List<string>> GetPermissonsOfUserAsync(string userId);
        Task<List<ApplicationRole>> GetRolesOfUserByUserIdAsync(string userId);
        Task<List<ApplicationIdentityUserClaim>> GetUserClaimsAsync(string userId);
        Task<PaginationResult<ApplicationUser>> GetUsers(int skip, int take, string search);
    }
}
