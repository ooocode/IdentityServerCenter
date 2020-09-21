using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerCenter.Shared.Database
{
    public static class UseDatabaseBuilderExtensions
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
