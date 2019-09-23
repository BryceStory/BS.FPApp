namespace FiiiPay.DTO.Security
{
    public class PreGetVerifyNewCellphoneCodeOM
    {
        /// <summary>
        /// 地区码，已经带上+，比如：+86
        /// </summary>
        public string PhoneCode { get; set; }

        /// <summary>
        /// 国家名，已经做多语言处理
        /// </summary>
        public string CountryName { get; set; }
    }
}
