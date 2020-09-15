using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.WebApi.ViewModels
{
    public class CreateOrUpdateUserViewModel
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        [Required]
        public string Name { get; set; }

        [DisplayName("密码")]
        [Required]
        public string Password { get; set; }
    }
}
