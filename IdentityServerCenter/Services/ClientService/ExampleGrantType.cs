using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    public class ExampleGrantType
    {
        public string Name { get; set; }

        public ICollection<string> GrantType { get; set; }

        public string Desc  { get; set; }


        public ExampleGrantType()
        {

        }

        public ExampleGrantType(string name, ICollection<string> grantType,string desc = null)
        {
            Name = name;
            GrantType = grantType;
            Desc = desc;
        }
    }
}
