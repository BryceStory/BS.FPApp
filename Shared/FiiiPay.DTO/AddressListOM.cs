using System.Collections.Generic;

namespace FiiiPay.DTO.Withdraw
{
    public class AddressListOM
    {
        public List<AddressListOMItem> List { get; set; }
    }

    public class AddressListOMItem
    {
        public long Id { get; set; }
        
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 地址，提交提币的时候，提交这个值，不要提交Id
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// tag
        /// </summary>
        public string Tag { get; set; }
    }
}
