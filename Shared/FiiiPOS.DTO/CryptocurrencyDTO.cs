using System;
using FiiiPay.Foundation.Entities.Enum;

namespace FiiiPOS.DTO
{
    public class CryptocurrencyDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        public CryptoStatus Status { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public Guid? IconURL { get; set; }
        /// <summary>
        /// 货币状态 0禁用 1可用
        /// </summary>
        public byte CryptoEnable { get; set; }
    }
}