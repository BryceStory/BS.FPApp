using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FiiiPay.DTO.Merchant
{
    public class GetListByCodeListIM
    {
        /// <summary>
        /// 15位的随机码
        /// </summary>
        [Required]
        public List<string> CodeList { get; set; }
    }
}
