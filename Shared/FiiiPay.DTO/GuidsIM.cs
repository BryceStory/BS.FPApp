using System;
using System.Collections.Generic;

namespace FiiiPay.DTO
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
