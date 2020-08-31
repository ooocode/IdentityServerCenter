using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityServerCenter.Database.Dtos.DictionaryDtos
{
    public class CreateOrUpdateDictionaryTypeDto
    {
        public long? Id { get; set; }


        [Required]
        public string Name { get; set; }


        public string Remark { get; set; }
    }
}
