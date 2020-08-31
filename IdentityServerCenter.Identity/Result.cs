using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Identity
{
    public static class ErrorMessageConsts
    {
        public const string ResourceNotFound = "资源不存在";
    }

    public class Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    public class DataResult<T> : Result
    {
        public T Data { get; set; }
    }

    public class PaginationResult<T> : Result
    {
        // <summary>
        /// 数据
        /// </summary>
        public IList<T> Rows { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 每一页记录数
        /// </summary>
        public int PageSize { get; set; }
    }





    ///// <summary>
    ///// 结果
    ///// </summary>
    //public class Result
    //{
    //    /// <summary>
    //    /// 是否成功
    //    /// </summary>
    //    public bool Succeeded { get; set; }

    //    /// <summary>
    //    /// 错误消息
    //    /// </summary>
    //    public string ErrorMessage { get; set; }
    //}


    ///// <summary>
    ///// 数据结果
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class DataResult<T> : Result
    //{
    //    /// <summary>
    //    /// 数据
    //    /// </summary>
    //    public T Data { get; set; }
    //}


    ///// <summary>
    ///// 分页结果
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class PaginationResult<T> : Result
    //{
    //    /// <summary>
    //    /// 数据
    //    /// </summary>
    //    public IList<T> Rows { get; set; }

    //    /// <summary>
    //    /// 总记录数
    //    /// </summary>
    //    public int Total { get; set; }

    //    /// <summary>
    //    /// 每一页记录数
    //    /// </summary>
    //    public int PageSize { get; set; }
    //}
}
