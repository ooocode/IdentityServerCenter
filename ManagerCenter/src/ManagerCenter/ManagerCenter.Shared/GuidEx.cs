// <copyright file="GuidEx.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.Shared
{
    using Snowflake.Core;

    /// <summary>
    /// GUID生成.
    /// </summary>
    public static class GuidEx
    {
        /// <summary>
        /// 使用snowFlake 产生唯一long型Id,自动递增，一般用于主键.
        /// </summary>
        /// <returns>long类型id.</returns>
        public static long NewGuid()
        {
            // var generator = new IdGenerator(0);
            // var id = generator.CreateId();
            var worker = new IdWorker(1, 1);
            long id = worker.NextId();

            // 加上当前线程id 避免多线程下相同
            return id;
        }
    }
}
