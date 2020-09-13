using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// mysql
        /// </summary>
        MySql,

        /// <summary>
        /// 微软sqlserver数据库
        /// </summary>
        MSSqlServer,

        /// <summary>
        /// sqlite本地数据库
        /// </summary>
        Sqlite
    }

    /// <summary>
    /// 数据库builder 扩展
    /// </summary>
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// 使用数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="connectString"></param>
        public static void UseDatabase(this DbContextOptionsBuilder builder, DatabaseType databaseType, string connectString)
        {
            switch (databaseType)
            {
                case DatabaseType.MySql:
                    builder.UseMySql(connectString, ctx => ctx.EnableRetryOnFailure());
                    break;
                case DatabaseType.MSSqlServer:
                    builder.UseSqlServer(connectString, ctx => ctx.EnableRetryOnFailure());
                    break;
                case DatabaseType.Sqlite:
                    builder.UseSqlite(connectString);
                    break;
                default:
                    break;
            }
        }
    }
}
