using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.HomePage
{
    public class ReOrder1IM
    {
        [Required]
        public List<int> IdList { get; set; }
    }
}
