using System.Collections.Generic;

namespace FiiiPay.DTO.Withdraw
{
    public class ListForManageWithdrawalAddressOM
    {
        public List<ListForManageWithdrawalAddressOMItem> List { get; set; }
    }

    public class ListForManageWithdrawalAddressOMItem
    {
        /// <summary>
        /// 币种Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 币的简称，比如：BTC
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 地址的数量
        /// </summary>
        public int AddressCount { get; set; }

        /// <summary>
        /// 是否需要Tag
        /// </summary>
        public bool NeedTag { get; set; }
    }
}
