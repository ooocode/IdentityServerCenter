using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    public class ClientDto : IdentityServer4.Models.Client
    {
        public int? Id { get; set; }
    }
}
