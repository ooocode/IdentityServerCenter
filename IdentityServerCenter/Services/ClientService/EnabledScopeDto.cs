using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    public class EnabledScopeDto
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        /// <summary>
        /// 类型标记   身份/API
        /// </summary>
        public string TypeTag  { get; set; }
    }
}
