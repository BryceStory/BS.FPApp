namespace FiiiPay.FiiiCoinWork.API.Models
{
    /// <summary>
    /// 验证PIN实体
    /// </summary>
    public class VerifyPINModel
    {
        /// <summary>
        /// 加密后的PIN
        /// </summary>
        public string EncryptPIN { get; set; }
    }
}