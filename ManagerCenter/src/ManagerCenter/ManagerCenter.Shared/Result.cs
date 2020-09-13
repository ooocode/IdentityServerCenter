// <copyright file="Result.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.Shared
{
    /// <summary>
    /// 结果.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets a value indicating whether 是否成功.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets 错误消息.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
