using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace FiiiPOS.Web.API.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class WebIdentity : ClaimsIdentity
    {
        public Guid Id { get; set; }

        public int CountryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="authenticationType"></param>
        public WebIdentity(IEnumerable<Claim> claims, string authenticationType) : base(claims, authenticationType)
        {
            foreach (Claim claim in claims)
            {
                if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    this.Id = Guid.Parse(claim.Value);
                }
                else if (claim.Type == ClaimTypes.Country)
                {
                    this.CountryId = int.Parse(claim.Value);
                }

            }
        }
    }
}