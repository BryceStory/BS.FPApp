using System;
using System.Collections.Generic;

namespace FiiiPay.Profile.API.Models
{
    public class GuidsIM
    {
        public GuidsIM()
        {
            this.Guids = new List<Guid>();
        }
        public List<Guid> Guids { get; set; }
    }
}
