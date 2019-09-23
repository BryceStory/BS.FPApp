using System;

namespace FiiiPay.Entities.EntitySet
{
    public class MerchantRegInfo
    {

        public Guid? Id { get; set; }
        public string Cellphone { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string Username { get; set; }
        public string MerchantName { get; set; }
        //public bool? IsVerified { get; set; }
        public long? POSId { get; set; }
        public int? CountryId { get; set; }
        /// <summary>
        /// 法币
        /// </summary>
        public string FiatCurrency { get; set; }
        /// <summary>
        /// 16的Id用于蓝牙广播，与Id一一对应，不能重复
        /// </summary>
        public string DeviceId { get; set; }

    }
}
