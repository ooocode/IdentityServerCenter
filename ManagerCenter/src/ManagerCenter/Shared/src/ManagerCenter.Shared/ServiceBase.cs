// <copyright file="ServiceBase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.Shared
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 服务基础类.
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// 成功结果.
        /// </summary>
        /// <returns>结果</returns>
        public virtual Result OkResult() => new Result { Succeeded = true };

        /// <summary>
        /// 失败结果
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public virtual Result FailedResult(string errorMessage) => new Result { Succeeded = false, ErrorMessage = errorMessage };

        public virtual Result FailedResult(Exception ex) => new Result { Succeeded = false, ErrorMessage = ex?.InnerException?.Message ?? ex.Message };

        public virtual DataResult<T> OkDataResult<T>(T data) => new DataResult<T> { Succeeded = true, Data = data };

        public virtual DataResult<T> FailedDataResult<T>(string errorMessage) => new DataResult<T> { Succeeded = false, ErrorMessage = errorMessage };

        public virtual DataResult<T> FailedDataResult<T>(Exception ex)
        {
            return new DataResult<T> { Succeeded = false, ErrorMessage = ex?.InnerException?.Message ?? ex.Message };
        }

        public virtual PaginationResult<T> OkPaginationResult<T>(IList<T> rows, int total, int pageSize) => new PaginationResult<T>
        {
            Succeeded = true,
            Rows = rows,
            Total = total,
            PageSize = pageSize,
        };

        public virtual PaginationResult<T> FailedPaginationResult<T>(string errorMessage) => new PaginationResult<T>
        {
            Succeeded = false,
            ErrorMessage = errorMessage,
        };

        public virtual PaginationResult<T> FailedPaginationResult<T>(Exception ex)
        {
            return new PaginationResult<T>
            {
                Succeeded = false,
                ErrorMessage = ex?.InnerException?.Message ?? ex?.Message,
            };
        }
    }
}
