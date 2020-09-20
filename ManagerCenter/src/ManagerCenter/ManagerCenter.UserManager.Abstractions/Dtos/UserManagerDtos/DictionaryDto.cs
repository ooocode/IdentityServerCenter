using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos
{
    public class DictionaryDto
    {
        public long Id { get; set; }

        public long DictionaryTypeId { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
