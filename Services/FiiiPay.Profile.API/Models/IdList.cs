using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.Profile.API.Models
{
    public class IdList
    {
        public List<Guid> Ids   { get; set; }
    }

    public class ReOrderIM
    {
        [Required]
        public List<int> IdList { get; set; }
    }
}
