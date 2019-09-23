using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FiiiPOS.API.Models
{
    public class GetSecurityCellphoneCodeModel
    {
        /// <summary>
        /// 设置不同的Code，以使同一业务下系统能分别发送和验证手机码，可空
        /// </summary>
        [MaxLength(50)]
        public string DivisionCode { get; set; }
    }
}