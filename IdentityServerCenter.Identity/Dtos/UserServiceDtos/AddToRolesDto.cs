using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityServerCenter.Database.Dtos.UserServiceDtos
{
    public class AddToRolesDto
    {
        [Required]
        public string UserId  { get; set; }


        [Required]
        public List<string> RoleIds  { get; set; }
    }
}
