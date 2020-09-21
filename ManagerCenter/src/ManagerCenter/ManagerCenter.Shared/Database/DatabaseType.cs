using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerCenter.Shared.Database
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
}
