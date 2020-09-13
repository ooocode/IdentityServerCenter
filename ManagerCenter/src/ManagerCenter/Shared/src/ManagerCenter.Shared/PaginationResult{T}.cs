// <copyright file="PaginationResult{T}.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// 分页数据结果.
    /// </summary>
    /// <typeparam name="T">泛型类型</typeparam>
    public class PaginationResult<T> : Result
    {
        /// <summary>
        /// Gets or sets 行数
        /// </summary>
        public IList<T> Rows { get; set; }

        /// <summary>
        /// Gets or sets 总记录数.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets 每一页记录数.
        /// </summary>
        public int PageSize { get; set; }
    }
}
