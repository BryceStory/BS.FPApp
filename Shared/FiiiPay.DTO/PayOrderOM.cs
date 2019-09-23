namespace FiiiPay.DTO.Order
{
    public class PayOrderOM
    {
        public string OrderId { get; set; }
        public string OrderNo { get; set; }
        public string Timestamp { get; set; }
        public string Amount { get; set; }
        /// <summary>
        /// 币种，比如：BTC
        /// </summary>
        public string Currency { get; set; }
    }
}
