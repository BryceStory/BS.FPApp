namespace FiiiPay.DTO.Order
{
    public class PrePayExistedOrderOM
    {
        /// <summary>
        /// 订单Id，跟客户端提供的订单Id一致，原值返回
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 币种，比如：BTC
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 用于如果余额不足，跳转到充值页面
        /// </summary>
        public int CoinId { get; set; }

        /// <summary>
        /// 金额，比如：0.01200000
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 商家名称，比如：星巴克深南店
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 用于此币种的余额
        /// </summary>
        public string Balance { get; set; }
    }
}
