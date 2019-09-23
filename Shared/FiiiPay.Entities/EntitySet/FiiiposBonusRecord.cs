using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.Entities.EntitySet
{
    public class FiiiposBonusRecord
    {
        public string MerchantName { get; set; }

        public decimal TotalCryptoAmount { get; set; }
        /// <summary>
        /// 作为下次获取商家分红详情的入参
        /// </summary>
        public Guid MerchantId { get; set; }

    }
}
