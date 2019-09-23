using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.API.Models
{
    public class CombineVerifyModel
    {
        [StringLength(10)]
        public string SMSCode { get; set; }

        [StringLength(10)]
        public string GoogleCode { get; set; }

        [StringLength(50)]
        public string IdNumber { get; set; }
    }
    public class BaseCombineVerifyModel
    {
        [StringLength(10)]
        public string SMSCode { get; set; }

        [StringLength(10)]
        public string GoogleCode { get; set; }
    }
    public class WithdrawCombineVerifyModel
    {
        [StringLength(10)]
        public string SMSCode { get; set; }

        [StringLength(10)]
        public string GoogleCode { get; set; }

        [StringLength(50)]
        public string IdNumber { get; set; }
        /// <summary>
        /// 传入币种CODE，设置不同的Code，以使同一业务下系统能分别发送和验证手机码，可空
        /// </summary>
        [MaxLength(50)]
        public string DivisionCode { get; set; }
    }
}