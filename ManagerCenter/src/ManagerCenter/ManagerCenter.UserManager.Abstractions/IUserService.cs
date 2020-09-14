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
        /// 创建或者更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<DataResult<string>> CreateOrUpdateUserAsync(ApplicationUser user);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result> DeleteUsersAsync(List<ApplicationUser> users);

        /// <summary>
        /// 通过id查找用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ApplicationUser> FindByIdAsync(string id);


        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<PaginationResult<ApplicationUser>> GetUsers(int skip, int take, string search);

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ApplicationUser> FindByUserNameAsync(string userName);


        /// <summary>
        /// 删除用户声明
        /// </summary>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        Task<Result> DeleteUserClaimsAsync(List<ApplicationIdentityUserClaim> userClaims);

     
        /// <summary>
        /// 获取用户拥有的permisson
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<string>> GetPermissonsOfUserAsync(string userId);

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ApplicationRole>> GetRolesOfUserByUserIdAsync(string userId);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto">dto</param>
        /// <returns>结果</returns>
        Task<Result> AddToRolesAsync(ApplicationUser user, List<string> roleIds);


        /// <summary>
        /// 创建或者更新用户声明
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        Task<Result> CreateOrUpdateUserClaimAsync(ApplicationIdentityUserClaim claim);

        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<ApplicationIdentityUserClaim>> GetUserClaimsAsync(string userId);

        /// <summary>
        /// 通过id获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        Task<ApplicationIdentityUserClaim> GetUserClaimByIdAsync(string userId, int claimId);
    }
}
