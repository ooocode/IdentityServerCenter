using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace IdentityServerCenter.Database.Models
{
    public class ApplicationIdentityRoleClaim : IdentityRoleClaim<string>
    {
        public ApplicationIdentityRoleClaim()
        {
            this.ClaimValueType = System.Security.Claims.ClaimValueTypes.String;
        }

        public ApplicationIdentityRoleClaim(string claimType, string claimValue)
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
            this.ClaimValueType = System.Security.Claims.ClaimValueTypes.String;
        }


        public ApplicationIdentityRoleClaim(string claimType, string claimValue, string claimValueType)
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
            this.ClaimValueType = claimValueType;
        }


        /// <summary>
        /// 值类型，默认 System.Security.Claims.ClaimValueTypes.String;
        /// </summary>
        public string ClaimValueType { get; set; }
    }
}
