using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    public class IdentityResourceDto : IdentityServer4.Models.IdentityResource
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IdentityResourceDto()
            : base()
        {

        }

        //     requested.
        public IdentityResourceDto(string name, IEnumerable<string> userClaims)
            : base(name, userClaims)
        {
            this.Name = name;
        }

        public IdentityResourceDto(string name, string displayName, IEnumerable<string> userClaims) :
            base(name, displayName, userClaims)
        {
            this.Name = name;
        }
    }
}
