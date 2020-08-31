using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    public class ApiResourceDto
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        //
        // 摘要:
        //     Indicates if this resource is enabled. Defaults to true.
        public bool Enabled { get; set; }

        //
        // 摘要:
        //     Display name of the resource.
        public string DisplayName { get; set; }
        //
        // 摘要:
        //     Description of the resource.
        public string Description { get; set; }
        //
        // 摘要:
        //     Specifies whether this scope is shown in the discovery document. Defaults to
        //     true.
        public bool ShowInDiscoveryDocument { get; set; }


        //API作用域
        // 摘要:
        // Models the scopes this API resource allows.
        public ICollection<string> Scopes { get; set; }
    }
}
