// <copyright file="DataResult{T}.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.Shared
{
    /// <summary>
    /// 带泛型的数据结果.
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class DataResult<T> : Result
    {
        /// <summary>
        /// Gets or sets 泛型数据.
        /// </summary>
        public T Data { get; set; }
    }
}
