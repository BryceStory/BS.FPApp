using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.DTO
{
    public class MerchantStoreTypeOM
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string Name_CN { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        public string Name_EN { get; set; }
    }
}
